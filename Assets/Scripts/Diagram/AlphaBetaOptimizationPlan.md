# αβ探索 高速化 原因分析と実装計画

## 目的

αβ探索を「ゲーム本体のコマンド実行パスを何度も通す実装」から、「探索専用の軽量な局面表現を in-place に更新して戻す実装」へ移行し、探索 1 ノードあたりの割り当て・経路探索・合法手生成コストを削減する。

## 現状コードから見える主なボトルネック

### 1. `MatchState.Capture()` / `Restore()` は探索ノード向きではない

`MatchState.Capture()` は `MatchMemento` を生成するだけだが、`MatchMemento` のコンストラクタは盤面・プレイヤー・ターンをすべて DeepCopy する。
さらに `MatchMemento.Board` / `Players` / `Turn` の getter も再度 DeepCopy を返すため、`Restore()` では「memento から取り出すたびにコピー」が発生する。

探索で「手を進めるたびに Capture、戻すたびに Restore」を行うと、各ノードで以下の割り当てが発生する。

- `BoardState.DeepCopy()` による `int[,]` と `Position[]` の clone
- `PlayerState.DeepCopy()` による `Dictionary<SkillSlotId, SkillState>` / `List<StatusState>` / `PlayerRuntimeState` の新規生成
- `TurnState.DeepCopy()` などの状態オブジェクト生成
- `Restore()` 内の `StateRestoredEvent(Capture())` による追加の Capture

これは Undo/Redo や UI 連携には安全だが、数万〜数百万ノードを触る探索ループには重すぎる。

### 2. 合法な壁配置の列挙が BFS を大量に呼ぶ

`PlaceWallLegalRule.EnumerateLegalPositions()` は全パターンを走査し、各パターンに対して `PlaceWallValidator.CanPlaceWall()` を呼ぶ。
`CanPlaceWall()` は候補壁を重ねた `WallCandidateGrid` を作ったうえで、先手・後手それぞれについて `Pathfinder.CanReachGoal()` を呼ぶ。
つまり、壁候補 1 個につき最大 2 回の BFS が走る。

αβ探索では各ノードで合法手生成が必要になるため、分岐数が多い壁配置のたびに BFS が掛け算される。

### 3. 評価関数で最短距離を都度 BFS するとノード数に比例して重い

`DistanceCalculator.Calculate()` は両プレイヤーについて `Pathfinder.GetShortestDistanceToGoal()` を呼ぶ。
`Pathfinder.GetShortestDistanceToGoal()` は BFS 用の `int[,] distance` と `Queue<Position>` を毎回生成し、全マスの初期化も行う。

評価関数が「自分の最短距離 - 相手の最短距離」を毎リーフ・毎内部ノードで計算する設計だと、評価 1 回につき最大 2 回の BFS と配列初期化が発生する。

### 4. コマンド/イベント/プレゼンテーション寄りの経路を探索に流用すると余計な処理が多い

`MatchCommandExecutor.Execute()` はコマンド実行後、ターン消費時に `DistanceCalculator.Calculate()`、勝敗解決、次ターン進行、履歴保存、イベント dispatch まで行う。
本番対局では必要だが、探索中の仮想手には不要な処理が多い。

探索で `IMatchCommand` を実行する構造をそのまま使うと、局面評価以外のイベント生成・履歴管理・状態復元もノード数分だけ膨らむ。

## 高速化方針

### 方針 A: 探索専用の軽量状態を導入する

本番用 `MatchState` を直接探索で破壊・復元しない。代わりに、探索開始時に一度だけ `SearchState` へ変換する。

推奨する `SearchState` の内容:

- 壁の有無を `int[,]` ではなく、壁セル用の `bool[]` / bitset で保持する
- 2 人分の pawn 位置を `Position` ではなく tile index で保持する
- 残り壁数・クールダウン・状態異常など、評価と合法手に必要な値だけを保持する
- `CurrentPlayerIndex` と ply をプリミティブ値で保持する

初期実装では既存仕様とのズレを避けるため、`SearchState` に `Apply(SearchMove)` / `Undo(SearchUndo)` を持たせる。
`SearchUndo` は「変更前の pawn index」「追加した壁セル」「変更前のターン値」など差分だけを持つ。

### 方針 B: DeepCopy ではなく make/unmake にする

αβ探索のノード展開は以下の形にする。

```csharp
foreach (var move in moveBuffer)
{
    var undo = state.Apply(move);
    score = -Search(state, depth - 1, -beta, -alpha);
    state.Undo(move, undo);
}
```

この構造により、探索ノードごとの `new MatchMemento` / `DeepCopy` / `Restore` をなくす。
`SearchUndo` も構造体化し、壁配置など可変長になりやすい差分は固定長配列または stackalloc 可能な小さい構造に寄せる。

### 方針 C: 合法手生成を段階的かつ探索用に分離する

既存の `LegalCommandEnumerator` は `IMatchCommand` を `List<IMatchCommand>` に詰めるため、探索では割り当てが多い。
探索用には `SearchMoveGenerator` を作り、以下を満たす。

- `SearchMove` は struct にする
- 呼び出し元が渡すバッファへ候補を書き込む
- 壁配置は全候補を毎回 `List` 化しない
- move ordering のため、候補に概算スコアを付けられるようにする

第一段階では既存 `MovePawnValidator` / `PlaceWallValidator` の結果と一致するテストを用意し、次段階で内部実装を高速化する。

### 方針 D: 経路探索をキャッシュ・再利用する

評価関数と壁合法性判定で BFS が集中するため、以下を導入する。

1. **BFS 作業領域の再利用**
   - `int[,] distance` / `bool[,] visited` / `Queue<Position>` を毎回 new しない
   - 盤サイズ固定なら一次元配列 `int[] distance`, `int[] queue` にする
   - 世代カウンタ方式で全マス初期化を避ける

2. **局面単位の距離キャッシュ**
   - Zobrist hash を `SearchState` に持たせる
   - `(hash, player)` -> shortest distance を Transposition Table または小さな LRU に保存
   - 壁配置・pawn 位置が変わったときに hash を差分更新する

3. **壁合法性判定の早期打ち切り**
   - 既存壁と干渉する候補は BFS 前に除外する
   - 候補壁がどちらの最短路にも影響しない場合は到達性 BFS を省略できる余地がある
   - まずは「到達性 BFS の作業領域再利用」と「直近候補結果キャッシュ」から始める

### 方針 E: 評価関数を差分評価に寄せる

最初の評価関数は以下で十分。

- `opponentDistance - selfDistance`
- 残り壁数差
- ゴール直前・相手封鎖のボーナス

ただし、距離は毎回 BFS で再計算せず、以下の順で改善する。

1. BFS 作業領域再利用のみで既存評価と同じ値を返す
2. `SearchState.Hash` による距離キャッシュを追加
3. `Apply/Undo` で「pawn が動いたプレイヤー」「壁が変化したか」を記録し、変化していない側の距離を再利用

### 方針 F: αβ探索そのものの枝刈り効率を上げる

状態更新が軽くなった後、以下を追加する。

- iterative deepening
- transposition table: exact / lower bound / upper bound
- principal variation move を最優先
- killer move / history heuristic
- 壁手より pawn 前進手・相手距離を伸ばす壁手を優先する軽量 ordering
- aspiration window
- 時間制限つき探索キャンセル

## 実装順序

### Phase 0: 計測基盤を作る

- αβ探索 1 手あたりのノード数、合法手生成回数、評価回数、BFS 回数、GC Alloc、経過 ms をログ出力する
- Unity Profiler だけに依存せず、EditMode テストや Development Build でも数値比較できる `SearchProfiler` を用意する
- 既存のランダム CPU と同じ初期局面で、深さ 1〜3 のベースラインを保存する

### Phase 1: 探索を本番コマンド実行パスから切り離す

- `AlphaBetaCpuAgentStrategy` は `MatchState` を一度だけ `SearchState` に変換する
- 探索中は `MatchCommandExecutor` / `MatchMemento` / `StateRestoredEvent` を使わない
- 最終的に選ばれた `SearchMove` だけを `UseSkillCommand` へ変換して本番の `IMatchCommandPort` に渡す

### Phase 2: make/unmake 対応

- `SearchState.Apply()` / `Undo()` を実装する
- pawn move、壁配置、ターン交代の差分を `SearchUndo` に記録する
- DeepCopy ベース探索と make/unmake 探索で、同じ深さ・同じ局面の最善手と評価値が一致するテストを作る

### Phase 3: 合法手生成の軽量化

- `SearchMoveGenerator` を追加する
- 既存 `LegalCommandEnumerator` と候補数・候補内容が一致するテストを作る
- `List<IMatchCommand>` 生成をやめ、再利用バッファに `SearchMove` を詰める
- 壁候補生成を pattern provider 依存から探索用の固定配列・事前計算へ移す

### Phase 4: BFS の割り当て削減

- `Pathfinder` 互換の探索用 `SearchPathfinder` を作る
- `visited` / `distance` / queue を再利用する
- 盤面を一次元 index 化する
- 既存 `Pathfinder` と到達可否・最短距離が一致するテストを追加する

### Phase 5: 評価キャッシュと TT

- `SearchState` に Zobrist hash を追加する
- 距離キャッシュを追加する
- transposition table を追加する
- iterative deepening と move ordering を導入する

### Phase 6: 安全性・回帰確認

- 壁でゴール経路を塞げないルールが維持されることをテストする
- Undo 後に hash、pawn、壁、ターン、スキル状態が完全に元へ戻ることをテストする
- 探索時間の上限を超えた場合も、直前の反復で得た合法手を返すことを確認する

## 優先度つき TODO

1. `SearchProfiler` をαβ探索へ接続し、探索 1 手ごとの計測ログを出す
2. `SearchState` / `SearchMove` / `SearchUndo` をαβ探索へ接続し、DeepCopy ベース探索との差分を比較する
3. αβ探索を `MatchState.Capture()` / `Restore()` 非依存にする
4. `SearchMoveGenerator` を追加し、`IMatchCommand` 生成を最終手だけに限定する
5. `SearchPathfinder` で BFS 作業領域を再利用する
6. 距離キャッシュ、transposition table、move ordering の順で枝刈り効率を上げる

## 期待効果

- ノードごとの DeepCopy と memento 生成がなくなり、GC Alloc が大幅に減る
- 評価関数と壁合法性判定の BFS コストが可視化され、再利用・キャッシュで削減できる
- 探索中のイベント生成・履歴保存・UI 更新用 memento 作成がなくなる
- 最終手だけを既存コマンドへ変換するため、ゲーム本体の安全な実行パスは維持できる
