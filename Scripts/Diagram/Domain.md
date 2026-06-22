# Domain

## State / Value Object クラス図

```mermaid
classDiagram
    class MatchState{
        +Board : BoardState
        +Turn : TurnState
        +Phase : MatchPhase
        +CurrentPlayerId : PlayerId
        +CurrentPlayer : PlayerState
        +IsInProgress : bool
        +GetPlayer(playerId) PlayerState
        +IsCurrentPlayer(playerId) bool
        +Capture() MatchMemento
        +Restore(memento) StateChangeResult
        +Start() StateChangeResult
        +Pause() StateChangeResult
        +Finish() StateChangeResult
    }

    class BoardState{
        +Pawns : IReadOnlyList~Position~
        +Grid : IReadOnlyIntGrid
        +GetPawn(playerId) Position
        +DeepCopy() BoardState
        +MovePawn(playerId, to) StateChangeResult
        +PlaceWall(walls) StateChangeResult
        +RemoveWall(walls) StateChangeResult
    }

    class PlayerState{
        +PlayerId : PlayerId
        +IsActive : bool
        +Skills : IReadOnlyDictionary~SkillSlotId, SkillState~
        +Statuses : IReadOnlyList~StatusState~
        +Runtime : PlayerRuntimeState
        +TryGetSkillBySlotId(slotId, out skill) bool
        +GetSkill(slotId) SkillState
        +UseSkill(slotId) StateChangeResult
        +HasStatus(statusId) bool
        +AddStatus(status) StateChangeResult
        +RemoveStatus(statusId) StateChangeResult
        +OnTurnStarted() StateChangeResult
        +DeepCopy() PlayerState
    }

    class TurnState{
        +CurrentPlayerId : PlayerId
        +CurrentTurn : int
        +NextTurn() void
        +PrevTurn() void
        +DeepCopy() TurnState
    }

    class PlayerRuntimeState{
        +CanAct : bool
        +CanMove : bool
        +CanPlaceWall : bool
        +CanUseSpecialSkill : bool
        +IsCpu : bool
        +IsAuto : bool
        +Reset() void
        +SetAuto(isAuto) void
        +ProhibitAction() void
        +ProhibitMove() void
        +ProhibitWallPlacement() void
        +ProhibitSpecialSkill() void
        +DeepCopy() PlayerRuntimeState
    }

    class SkillState{
        +SkillId : SkillId
        +RemainingUses : int?
        +CoolDownTurns : int
        +CoolDownRemaining : int
        +Charge : int?
        +CanUse() bool
        +Use() void
        +Advance() void
        +Consume(amount) void
        +Recover(amount) void
        +DeepCopy() SkillState
    }

    class StatusState{
        +StatusId : StatusId
        +RemainingTurns : int?
        +CoolDownTurns : int
        +CoolDownRemaining : int
        +IsExpired : bool
        +IsReady : bool
        +Advance() void
        +RefreshRemaining(amount) void
        +DeepCopy() StatusState
    }

    class StateChangeResult{
        +Events : IReadOnlyList~IMatchEvent~
        +NoChange() StateChangeResult$
        +Changed(event) StateChangeResult$
        +Merge(results) StateChangeResult$
    }

    class MatchMemento{
        +Board : BoardState
        +Players : IReadOnlyList~PlayerState~
        +Turn : TurnState
        +Phase : MatchPhase
    }

    class MatchHistory{
        +Push(memento) void
        +TryUndo(out memento) bool
        +TryRedo(out memento) bool
    }

    class Position{
        <<struct>>
        +X : int
        +Y : int
    }

    class PlayerId{
        <<record>>
        +Value : int
        +FirstPlayer : PlayerId$
        +SecondPlayer : PlayerId$
        +Opponent : PlayerId
        +ToIndex() int
    }

    class SkillId{
        <<record>>
        +Value : string
        +Of(value) SkillId$
    }

    class SkillSlotId{
        <<record>>
        +Value : int
        +ToIndex() int
    }

    class DistanceSnapshot{
        <<struct>>
        +FirstDistance : int
        +SecondDistance : int
        +GetDistance(playerId) int
    }

    MatchState "1" *-- "1" BoardState
    MatchState "1" *-- "2" PlayerState
    MatchState "1" *-- "1" TurnState
    MatchState ..> MatchMemento : capture/restore
    MatchHistory o-- MatchMemento
    BoardState "1" *-- "2" Position : pawns
    BoardState --> IReadOnlyIntGrid
    PlayerState "1" *-- "0..*" SkillState
    PlayerState "1" *-- "0..*" StatusState
    PlayerState "1" *-- "1" PlayerRuntimeState
    PlayerState --> PlayerId
    SkillState --> SkillId
    StatusState --> StatusId
    TurnState --> PlayerId
    StateChangeResult --> IMatchEvent
```

## Rule / Service クラス図

```mermaid
classDiagram
    class GoalResolver{
        +IsGoal(board, playerId, position) bool
        +HasPlayerReachedGoal(board, playerId) bool
        +FindWinner(board) PlayerId
    }

    class CheckmateResolver{
        +IsCheckmate(match, playerId, nextPlayerId, distances) bool
        +FindWinner(match, distances) PlayerId
    }

    class MatchResultResolver{
        +Resolve(state, distances) IReadOnlyList~IMatchEvent~
    }

    class Pathfinder{
        +CanReachGoal(grid, pawns, playerId) bool
        +FindShortestDistance(grid, pawns, playerId) int
    }

    class PlaceWallValidator{
        +CanPlaceWall(state, pattern) bool
    }

    class SkillAvailabilityValidator{
        +Evaluate(state, playerId, skillSlotId) SkillAvailabilityResult
    }

    class DistanceCalculator{
        +Calculate(state) DistanceSnapshot
    }

    class BoardGeometry{
        <<static>>
    }

    class BoardDirections{
        <<static>>
    }

    class PawnHelper{
        <<static>>
        +GetPawnPosition(board, playerId) Position
    }

    MatchResultResolver --> GoalResolver
    MatchResultResolver --> CheckmateResolver
    DistanceCalculator --> Pathfinder
    DistanceCalculator --> GoalResolver
    PlaceWallValidator --> Pathfinder
    SkillAvailabilityValidator --> ISkillDefinitionRegistry
    Pathfinder ..> GoalResolver
    CheckmateResolver ..> BuiltInSkillSlotIds
```

## Enum / Value 一覧

| 種別 | 値 |
|---|---|
| `MatchPhase` | Ready, InProgress, Paused, Finished |
| `PlayerSide` | First, Second |
| `WallDirection` | Horizontal, Vertical |
| `SkillActivationType` | Immediate, BoardTarget |
| `SkillTargetKind` | None, Tile, Wall |
| `SkillTargetPlayerPolicy` | Self, Opponent, Any |
| `StatusEffectId` | CannotAct, ProbabilisticCannotAct, CannotMovePawn, CannotPlaceWall, CannotUseSpecialSkill, RecoveryWall, DamageWall |
| `StatusReapplyPolicy` | Ignore, Refresh, Stack |
| `StatusId` | Sleep, Paralysis, SealMovePawn, SealPlaceWall, SealSpecialSkill, RecoveryWall, DamageWall |
