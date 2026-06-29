# CPU 探索速度改善タスクリスト

## 目的

CPU の αβ 探索を、ゲーム本体のコマンド実行・イベント通知・DeepCopy 復元に依存した重い探索から、探索専用の軽量な局面表現を in-place に更新して戻す探索へ段階的に移行する。

主な削減対象は以下の 4 点。

- 探索ノードごとの `MatchState.Capture()` / `Restore()` / `DeepCopy()` による GC Alloc
- 合法な壁配置列挙時に候補ごとに発生する BFS
- 評価関数で毎回発生する最短距離 BFS と配列初期化
- 探索中には不要なコマンド履歴、イベント dispatch、UI 更新向け memento 生成

## 現状メモ

- `SearchProfiler` は追加済み。ノード数、BFS 回数、GC Alloc、経過時間を記録できる。
- `SearchState` / `SearchMove` / `SearchUndo` は追加済み。pawn 移動・壁配置の Apply/Undo 基本動作は存在する。
- 現在の `AlphaBetaCpuAgentStrategy` はまだ `IMatchCommand` 候補と `CpuCommandSimulator` / `MatchState` ベースで探索しているため、探索専用状態を活かし切れていない。
- 既存の詳細分析は `Assets/Scripts/Diagram/AlphaBetaOptimizationPlan.md` を参照する。

## 優先度つき TODO

### P0: 計測を探索本体へ接続する

- [ ] `AlphaBetaCpuAgentStrategy` の探索開始時に `SearchProfiler.Begin()`、終了時に `End()` を呼ぶ。
- [ ] root、内部ノード、評価呼び出しで `RecordNode()` を呼び、深さごとのノード数をログに残す。
- [ ] `Pathfinder` 経由の BFS 回数が探索 1 手単位で見えるよう、既存 DI 経路で profiler を渡す。
- [ ] 初期局面・代表局面で深さ 1〜3 のベースラインを保存する。
- [ ] 計測ログには `depth`, `nodes`, `bfs`, `gcAlloc`, `elapsedMs`, `bestScore`, `timeout` を含める。

### P1: αβ探索を `SearchState` ベースへ移行する

- [ ] 探索開始時に `MatchState` から `SearchState` を 1 回だけ生成する。
- [ ] 探索中は `CpuCommandSimulator`、`MatchCommandExecutor`、`MatchMemento`、`StateRestoredEvent` を使わない。
- [ ] `SearchState.Apply()` / `Undo()` を αβ 探索ループへ接続する。
- [ ] 最終的に選ばれた 1 手だけ `SearchMove.ToUseSkillCommand()` で本番コマンドへ変換する。
- [ ] 既存 `IMatchCommand` ベース探索と、同一局面・同一深さの最善手または評価値が一致する回帰テストを追加する。

### P2: 探索用合法手生成を追加する

- [ ] `SearchMoveGenerator` を追加し、呼び出し元が渡す再利用バッファへ `SearchMove` を書き込む。
- [ ] `SearchMove` は struct のまま維持し、探索中に `IMatchCommand` / `List<IMatchCommand>` を生成しない。
- [ ] pawn 移動候補を既存 `MovePawnValidator` と一致させるテストを追加する。
- [ ] 壁配置候補を既存 `PlaceWallValidator` / `LegalCommandEnumerator` と一致させるテストを追加する。
- [ ] 壁パターンは探索開始時に固定配列へ事前計算し、ノードごとの pattern provider 走査を避ける。
- [ ] 候補には軽量な ordering score を持たせ、αβ の枝刈り効率を上げる準備をする。

### P3: BFS の割り当てと初期化を削減する

- [ ] 探索用 `SearchPathfinder` を追加する。
- [ ] `int[,] distance`、`bool[,] visited`、`Queue<Position>` の毎回生成をやめ、一次元配列と queue バッファを再利用する。
- [ ] 世代カウンタ方式で盤面全体の初期化を避ける。
- [ ] `Pathfinder` と `SearchPathfinder` の到達可否・最短距離が一致するテストを追加する。
- [ ] 壁合法性判定では既存壁との衝突や盤外チェックを BFS 前に終わらせる。
- [ ] 候補壁が明らかに経路へ影響しないケースを検出できるか検証し、可能なら BFS を省略する。

### P4: 評価関数と距離計算をキャッシュする

- [ ] `SearchState` に Zobrist hash を追加し、Apply/Undo で差分更新する。
- [ ] `(hash, player)` をキーにした最短距離キャッシュを追加する。
- [ ] pawn が動いたプレイヤー、壁が変化したかを `SearchUndo` または局面更新結果に記録する。
- [ ] 変化していない側の距離を再利用する。
- [ ] キャッシュヒット率、キャッシュサイズ、衝突時の挙動を計測ログへ追加する。

### P5: αβ探索の枝刈り効率を上げる

- [ ] iterative deepening で前回反復の principal variation を最優先する。
- [ ] transposition table を追加し、exact / lower bound / upper bound を扱う。
- [ ] killer move と history heuristic を追加する。
- [ ] pawn 前進手、勝利直前手、相手距離を伸ばす壁手を優先する ordering を追加する。
- [ ] aspiration window を導入する。
- [ ] 時間切れ時は、完了済みの最深反復で得た合法手を必ず返す。

### P6: 安全性・回帰確認を固める

- [ ] Undo 後に pawn、壁、ターン、hash、残り壁数、クールダウン、状態異常が完全に元へ戻ることをテストする。
- [ ] 壁でどちらかのゴール経路を完全に塞げないルールが維持されることをテストする。
- [ ] ランダム局面を生成し、既存 validator と探索用 generator / pathfinder の結果を比較する property test を追加する。
- [ ] 探索時間上限が極端に短い場合でも null ではなく合法手へフォールバックすることを確認する。
- [ ] Development Build または EditMode テストで、変更前後の `nodes/sec`、`bfs/node`、`gcAlloc/decision` を比較する。

## 完了条件

- 深さ 2〜3 の代表局面で、CPU 1 手あたりの GC Alloc が現状より大幅に減っている。
- `bfs/node` が計測でき、SearchPathfinder と距離キャッシュ導入後に低下している。
- 既存の合法手・壁到達性ルールと探索用実装の結果がテストで一致している。
- 時間制限つき探索でフレームを長時間ブロックせず、常に合法手を返せる。
