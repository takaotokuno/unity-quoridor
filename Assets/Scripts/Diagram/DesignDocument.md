# Quoridor ボードゲーム 設計書（ソースコード準拠版）

> この文書は `Quoridor/Scripts` 配下の現行ソースコードを基準に更新しています。詳細なクラス関係は同じ `Diagram` フォルダ内の各クラス図を参照してください。

---

## 1. 全体アーキテクチャ

本プロジェクトは、Unity の実行基盤上に **Domain / Application / Presentation / UnityIntegration / GameFlow** を分ける構成です。

```text
[GameFlow]
  GameDirector / BoardGamePort / GameRequest / GameResponse
        |
        v
[Match Application]
  MatchFactory / CommandPort / Executor / EventBus / InputPort / Observers
        |
        v
[Match Domain]
  MatchState / BoardState / PlayerState / Skill / Status / Rule / Event
        ^
        |
[Presentation]
  Presenter / ViewModel / View / Input / Interaction / CPU Agent
        |
        v
[UnityIntegration]
  Sound / TimeEffects / Background / Novel / Lifetime / Logger / Random
```

### 1.1 レイヤー方針

- **Domain** はゲーム状態、値オブジェクト、ルール、スキル・ステータス定義、ドメインイベントを持ちます。
- **Application** はコマンド受付、コマンド実行、イベント配信、入力変換、Factory による依存組み立てを担当します。
- **Presentation** は Unity View、ViewModel、Presenter、入力状態、インタラクション状態、CPU Agent を担当します。
- **UnityIntegration** は Sound / TimeEffects / Background / Novel / Logger / Random など外部サービスをインターフェース経由で提供します。
- **GameFlow** はゲーム全体のセッション生成・リセットの入口です。

### 1.2 主な設計パターン

| パターン | 現行コードでの用途 |
|---|---|
| Command | `IMatchCommand` と各 `*Command` でプレイヤー操作・システム操作を表現 |
| Visitor | `IMatchCommand.Execute(ICommandVisitor)` で `CommandVisitor` に処理を委譲 |
| Observer / Pub-Sub | `IMatchEventBus` が `IMatchEvent` を型別に購読者へ配信 |
| Memento | `MatchState.Capture()` / `MatchHistory` / `StateRestoredEvent` による Undo/Redo |
| Factory | `MatchFactory` と下位 Factory が状態・コマンド・表示・CPU を組み立て |
| MVVM寄り | Presenter が ViewModel を更新し、View が描画・フィードバックを担当 |
| Strategy | CPU Agent の `ICpuAgentStrategy`、スキル効果 Composer、合法手 Rule を差し替え |
| Registry | Skill / Status の Definition・Composer・Rule・Processor を ID で解決 |

---

## 2. レイヤー別責務

### 2.1 Domain

| 領域 | 主なクラス | 責務 |
|---|---|---|
| State | `MatchState`, `BoardState`, `PlayerState`, `TurnState` | 対局状態、盤面、プレイヤー、ターンの保持と状態変更 |
| Runtime | `PlayerRuntimeState`, `SkillState`, `StatusState` | ターン中の行動可否、スキル使用回数・クールダウン、状態異常の残りターン管理 |
| ValueObject | `PlayerId`, `SkillId`, `SkillSlotId`, `Position`, `DistanceSnapshot` | ドメインで使う値の制約・表現 |
| Rule | `GoalResolver`, `CheckmateResolver`, `Pathfinder`, `PlaceWallValidator`, `SkillAvailabilityValidator` | 勝敗、到達可能性、壁設置、スキル使用可否などの判定 |
| Skill | `SkillDefinition`, `SkillEffectResolver`, `ISkillEffectComposer`, `ISkillLegalRule` | スキル定義、効果生成、対象合法性判定 |
| Status | `StatusDefinition`, `StatusEffectProcessorRegistry`, `IStatusEffectProcessor` | ステータス定義、再付与方針、ターン開始時効果処理 |
| History | `MatchMemento`, `MatchHistory` | Undo / Redo 用の状態スナップショット管理 |
| Event | `IMatchEvent` と各イベント | 状態変化・入力・距離更新・拒否理由などを表現 |

### 2.2 Application

| 領域 | 主なクラス | 責務 |
|---|---|---|
| Command Handling | `CommandVisitor`, `UseSkillCommandHandler`, `MatchControlCommandHandler` | Command を具体処理へルーティングし、状態変更結果を `CommandResult` に変換 |
| Command Service | `MatchCommandPort`, `MatchCommandExecutor` | コマンドをメインスレッドのキューで順次実行し、履歴記録・ターン終了処理・イベント配信を行う |
| Event Service | `MatchEventBus`, `MatchEventInterpreter`, `MatchEventLogObserver` | イベント配信、音・演出・ログへの橋渡し |
| Input Service | `MatchInputPort`, `MatchInputStateUpdater`, `MatchInputReleaseValidator`, `MatchInputCommandDispatcher`, `SkillSelectionController` | View からの入力 Intent を検証し Command または選択状態更新に変換 |
| Turn / Status | `TurnAdvancer`, `StatusApplicator`, `StatusEffectApplicator`, `DistanceCalculator` | ターン進行、状態異常の付与・適用、ゴール距離計算 |
| Factory | `MatchFactory`, `MatchStateFactory`, `MatchPresentationFactory`, `MatchCommandPortFactory`, `CpuAgentFactory` | 対局セッション生成に必要な依存を構築 |

### 2.3 Presentation

| 領域 | 主なクラス | 責務 |
|---|---|---|
| Presentation Root | `MatchPresentation` | Presenter 群と ObjectLayoutView のライフサイクル管理 |
| Presenter | `BoardPresenter`, `TurnPanelPresenter`, `MatchControlPresenter`, `SkillButtonPresenter`, `StatusPanelPresenter` | EventBus を購読し ViewModel / View を更新 |
| ViewModel | `BoardCellViewModel`, `ButtonViewModel`, `SkillButtonViewModel`, `StatusPanelViewModel` | 表示状態データの保持 |
| View | `BoardView`, `TileView`, `WallView`, `PawnView`, `SkillButtonView`, `StatusPanelView`, `TurnPanelView`, `MatchControlView` など | Unity 上の描画、アニメーション、入力イベント通知 |
| Input | `InputTarget`, `InputIntent`, `InputStateStore` | 入力対象と Hover / Press / Release 等の状態管理 |
| Interaction | `InteractionStateStore`, `InteractionStateCalculator`, `InteractionStateProjector`, `SkillSelectionStore` | 盤面・ボタンの有効/強調/選択状態を投影 |
| CPU Agent | `MlAgentsCpuAgent`, `ICpuAgentStrategy`, `LegalCommandEnumerator` | CPU プレイヤーの合法コマンド生成と発行 |

### 2.4 UnityIntegration / GameFlow

| 領域 | 主なクラス | 責務 |
|---|---|---|
| GameFlow | `GameDirector`, `BoardGamePort`, `NewSessionRequest`, `ResetRequest` | 対局セッションの作成・リセット・外部入口 |
| Lifetime | `GameLifetimeScope` | VContainer による依存登録 |
| Sound | `SoundManager`, `SoundCatalog`, `BgmEntry`, `SeEntry` | BGM / SE 再生と音量制御 |
| Time Effects | `TimeEffectManager` | ヒットストップ等の時間演出 |
| Background | `BackgroundEffectManager`, `BackgroundEffectCatalog` | 背景エフェクトのプリセット・強度制御 |
| Novel | `NovelGamePort`, `ScenarioCatalog` | ノベルゲーム側シナリオへのジャンプ |
| Debug / Utility | `UnityGameLogger`, `UnityRandomProvider`, `BoardClickInput` | ログ、乱数、Unity クリック入力補助 |

---

## 3. 主要データフロー

### 3.1 セッション生成

```text
GameDirector.DispatchRequest(NewSessionRequest)
  -> MatchFactory.Create(MatchSetting)
  -> MatchConfigMapper.ToStateConfig / ToPresentationConfig
  -> MatchStateFactory.Create
  -> MatchEventBus / MatchEventInterpreter / MatchEventLogObserver
  -> MatchCommandPortFactory.Create
  -> CpuAgentFactory.Create
  -> MatchPresentationFactory.Create
  -> MatchSession
```

### 3.2 入力からコマンド実行まで

```text
View(IUserInteractable)
  -> MatchInputPort.Handle(InputTarget, InputIntent)
  -> MatchInputStateUpdater.Apply
  -> MatchInputReleaseValidator.CanAccept
  -> MatchInputCommandDispatcher.Dispatch
  -> MatchCommandPort.DispatchCommand
  -> MatchCommandExecutor.Execute
  -> command.Execute(CommandVisitor)
  -> UseSkillCommandHandler / MatchControlCommandHandler / BoardState direct command
  -> StateChangeResult / CommandResult
  -> TurnEndedEvent / DistanceUpdatedEvent / MatchResultResolver / TurnAdvancer
  -> MatchHistory.Push
  -> IMatchEvent.Dispatch(IMatchEventBus)
  -> Presenter / Interpreter / LogObserver / CPU Agent
```

### 3.3 ユーザー操作と Command の対応

| 入力 | 主な変換先 |
|---|---|
| タイル Release | `UseSkillCommand`（通常移動または選択中スキルの対象） |
| 壁セル Release | `UseSkillCommand`（通常壁設置または選択中スキルの対象） |
| スキルボタン Release | `SkillSelectionController.Toggle/Clear` または対象なし即時 `UseSkillCommand` |
| 投了ボタン Release | `ResignCommand` |
| スキップボタン Release | `SkipCommand` |
| Hover / Press / MouseOut | `InputReceivedEvent` と InteractionState 更新 |

---

## 4. 勝敗・ターン・履歴

- `MatchCommandExecutor` はターン消費コマンド後に `TurnEndedEvent` と `DistanceUpdatedEvent` を発行します。
- `MatchResultResolver` は `GoalResolver` によるゴール到達と `CheckmateResolver` によるチェックメイトを判定し、勝者決定・試合終了イベントを生成します。
- 試合が継続中なら `TurnAdvancer.AdvanceToNextActableTurn()` が次に行動可能なプレイヤーへ進め、ターン開始時に `StatusEffectApplicator` を適用します。
- `UndoCommand` / `RedoCommand` は `MatchCommandExecutor` が直接処理し、`MatchHistory` から memento を復元して `StateRestoredEvent` を発行します。

---

## 5. 現行コード上の注意点

- `UndoCommand.Execute()` と `RedoCommand.Execute()` は例外を投げますが、通常経路では `MatchCommandExecutor` が Visitor に渡す前に専用処理します。
- `PlaceWallCommand` / `MovePawnCommand` は存在しますが、通常の UI 入力では `UseSkillCommand` と組み込みスキル定義を通して移動・壁設置が行われます。
- スキル・ステータス・表示定義は ScriptableObject Catalog から Definition / ViewEntry に変換または検索されます。
- CPU プレイヤーは `PlayerConfig.IsCpu` が true の場合に Factory で生成され、EventBus と CommandPort に接続されます。
