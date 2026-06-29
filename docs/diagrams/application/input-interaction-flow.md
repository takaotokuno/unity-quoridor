# Input / Interaction Flow

## 入力系クラス図

```mermaid
classDiagram
    class IUserInteractable{
        <<interface>>
        +Hovered() void
        +Pressed() void
        +Released() void
        +MouseOut() void
    }

    class InputTarget{
        +Kind : InputTargetKind
        +Position : Position?
        +PlayerId : PlayerId
        +SkillSlotId : SkillSlotId
        +ButtonId : ButtonId
    }

    class InputIntent{
        <<enum>>
        Hover
        Press
        Release
        MouseOut
    }

    class InputTargetKind{
        <<enum>>
        Tile
        Wall
        SkillButton
        ResignButton
        SkipButton
    }

    class InputStateStore{
        +HoveredTarget : InputTarget
        +PressedTarget : InputTarget
        +ApplyIntent(target, intent) StateChangeResult
    }

    class MatchInputPort{
        -_stateUpdater : MatchInputStateUpdater
        -_releaseValidator : MatchInputReleaseValidator
        -_commandDispatcher : MatchInputCommandDispatcher
        +Handle(target, intent) IMatchResponse
    }

    class MatchInputStateUpdater{
        +Apply(target, intent) void
    }

    class MatchInputReleaseValidator{
        +CanAccept(target, intent) bool
    }

    class MatchInputCommandDispatcher{
        +Dispatch(target, intent) IMatchResponse
    }

    class MatchInputRejectionDispatcher{
        +Reject(target, intent, reason) IMatchResponse
    }

    class SkillSelectionController{
        +Toggle(playerId, skillSlotId) void
        +Clear() void
    }

    IUserInteractable ..> MatchInputPort : sends target+intent
    MatchInputPort --> MatchInputStateUpdater
    MatchInputPort --> MatchInputReleaseValidator
    MatchInputPort --> MatchInputCommandDispatcher
    MatchInputStateUpdater --> InputStateStore
    MatchInputStateUpdater --> IMatchEventBus
    MatchInputReleaseValidator --> InteractionStateStore
    MatchInputReleaseValidator --> MatchInputRejectionDispatcher
    MatchInputCommandDispatcher --> IMatchCommandPort
    MatchInputCommandDispatcher --> ISkillDefinitionRegistry
    MatchInputCommandDispatcher --> SkillSelectionController
    MatchInputCommandDispatcher --> MatchInputRejectionDispatcher
    MatchInputCommandDispatcher ..> UseSkillCommand
    MatchInputCommandDispatcher ..> ResignCommand
    MatchInputCommandDispatcher ..> SkipCommand
```

## Interaction State クラス図

```mermaid
classDiagram
    class InteractionState{
        +IsInteractable : bool
        +IsHighlighted : bool
        +IsEmphasized : bool
        +IsPressed : bool
    }

    class InteractionStateStore{
        +GetBoardCellState(position) InteractionState
        +GetSkillState(playerId, slotId) InteractionState
        +GetButtonState(buttonId) InteractionState
        +RefreshAll() void
    }

    class InteractionStateCalculator{
        -_skillDefinitionRegistry : ISkillDefinitionRegistry
        +CalculateBoardCell(state, target) InteractionState
        +CalculateSkillButton(state, playerId, slotId) InteractionState
        +CalculateControlButton(state, buttonId) InteractionState
    }

    class InteractionStateProjector{
        +Notify(InputReceivedEvent) void
        +Notify(SkillSelectionChangedEvent) void
        +Notify(TurnStartedEvent) void
        +SubscribeTo(eventBus) void
    }

    class SkillSelectionStore{
        +SelectedPlayerId : PlayerId
        +SelectedSkillSlotId : SkillSlotId
        +Select(playerId, slotId) void
        +Clear() void
    }

    InteractionStateStore --> MatchState
    InteractionStateStore --> InteractionStateCalculator
    InteractionStateStore --> SkillSelectionStore
    InteractionStateProjector --> InteractionStateStore
    InteractionStateProjector --> IMatchEventBus
    InteractionStateCalculator --> ISkillDefinitionRegistry
```

## シーケンス

```mermaid
sequenceDiagram
    participant View as View(IUserInteractable)
    participant Input as MatchInputPort
    participant State as MatchInputStateUpdater
    participant Validator as MatchInputReleaseValidator
    participant Dispatcher as MatchInputCommandDispatcher
    participant CommandPort as MatchCommandPort
    participant Bus as MatchEventBus

    View->>Input: Handle(target, intent)
    Input->>State: Apply(target, intent)
    State->>Bus: InputReceivedEvent
    alt intent is Release
        Input->>Validator: CanAccept(target, intent)
        alt accepted
            Input->>Dispatcher: Dispatch(target, intent)
            Dispatcher->>CommandPort: DispatchCommand(command)
        else rejected
            Dispatcher->>Bus: InputRejectedEvent
        end
    end
```

## Release 時の変換

| Target | 変換 |
|---|---|
| Tile / Wall | 選択中スキルがあればその `SkillSlotId`、なければ通常移動/通常壁設置の `UseSkillCommand` |
| SkillButton | 対象なし即時スキルなら `UseSkillCommand`、対象指定スキルなら `SkillSelectionController.Toggle` |
| ResignButton | `ResignCommand` |
| SkipButton | `SkipCommand` |
