# Status システム

## Status クラス図

```mermaid
classDiagram
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

    class StatusDefinition{
        +StatusId : StatusId
        +ReapplyPolicy : StatusReapplyPolicy
        +Effects : IReadOnlyList~StatusEffectDefinition~
    }

    class StatusEffectDefinition{
        +EffectId : StatusEffectId
        +Parameters : IReadOnlyDictionary~string, int~
        +GetInt(key, defaultValue) int
    }
    StatusDefinition "1" *-- "1..*" StatusEffectDefinition

    class IStatusDefinitionRegistry{
        <<interface>>
        +Find(statusId) StatusDefinition
    }

    class StatusDefinitionRegistry{
        -_definitions : Dictionary~StatusId, StatusDefinition~
        +Find(statusId) StatusDefinition
    }
    IStatusDefinitionRegistry <|.. StatusDefinitionRegistry
    StatusDefinitionRegistry o-- StatusDefinition

    class StatusCatalog{
        <<ScriptableObject>>
        +Entries : IReadOnlyList~StatusCatalogEntry~
    }

    class StatusCatalogEntry{
        <<Serializable>>
        +StatusId : StatusId
        +ReapplyPolicy : StatusReapplyPolicy
        +Effects : IReadOnlyList~StatusEffectCatalogEntry~
    }

    class StatusEffectCatalogEntry{
        <<Serializable>>
        +EffectId : StatusEffectId
        +Parameters : IReadOnlyList~StatusParameterEntry~
    }

    class StatusParameterEntry{
        <<Serializable>>
        +Key : string
        +Value : int
    }

    class StatusCatalogConverter{
        <<static>>
        +Convert(catalog) IReadOnlyList~StatusDefinition~
    }

    StatusCatalog "1" *-- "0..*" StatusCatalogEntry
    StatusCatalogEntry "1" *-- "0..*" StatusEffectCatalogEntry
    StatusEffectCatalogEntry "1" *-- "0..*" StatusParameterEntry
    StatusCatalogConverter ..> StatusCatalog
    StatusCatalogConverter ..> StatusDefinition
```

## Status 適用 / Processor クラス図

```mermaid
classDiagram
    class StatusApplicator{
        -_statusDefinitionRegistry : IStatusDefinitionRegistry
        +Apply(player, newStatus) StateChangeResult
    }

    class StatusEffectApplicator{
        -_definitionRegistry : IStatusDefinitionRegistry
        -_processorRegistry : IStatusEffectProcessorRegistry
        +Apply(player) IReadOnlyList~IMatchEvent~
    }

    class IStatusEffectProcessor{
        <<interface>>
        +EffectId : StatusEffectId
        +Apply(context) IReadOnlyList~IMatchEvent~
    }

    class IStatusEffectProcessorRegistry{
        <<interface>>
        +Find(effectId) IStatusEffectProcessor
    }

    class StatusEffectProcessorRegistry{
        -_processors : Dictionary~StatusEffectId, IStatusEffectProcessor~
        +Find(effectId) IStatusEffectProcessor
    }
    IStatusEffectProcessorRegistry <|.. StatusEffectProcessorRegistry

    class StatusEffectContext{
        +PlayerId : PlayerId
        +Player : PlayerState
        +StatusId : StatusId
        +Effect : StatusEffectDefinition
    }

    class CannotActStatusEffectProcessor
    class CannotMovePawnStatusEffectProcessor
    class CannotPlaceWallStatusEffectProcessor
    class CannotUseSpecialSkillStatusEffectProcessor
    class ProbabilisticCannotActStatusEffectProcessor
    class RecoveryWallStatusEffectProcessor
    class DamageWallStatusEffectProcessor

    IStatusEffectProcessor <|.. CannotActStatusEffectProcessor
    IStatusEffectProcessor <|.. CannotMovePawnStatusEffectProcessor
    IStatusEffectProcessor <|.. CannotPlaceWallStatusEffectProcessor
    IStatusEffectProcessor <|.. CannotUseSpecialSkillStatusEffectProcessor
    IStatusEffectProcessor <|.. ProbabilisticCannotActStatusEffectProcessor
    IStatusEffectProcessor <|.. RecoveryWallStatusEffectProcessor
    IStatusEffectProcessor <|.. DamageWallStatusEffectProcessor

    StatusApplicator --> IStatusDefinitionRegistry
    StatusEffectApplicator --> IStatusDefinitionRegistry
    StatusEffectApplicator --> IStatusEffectProcessorRegistry
    IStatusEffectProcessor ..> StatusEffectContext
    StatusEffectContext --> PlayerState
    StatusEffectContext --> StatusEffectDefinition
```

## Status View クラス図

```mermaid
classDiagram
    class StatusViewCatalog{
        <<ScriptableObject>>
        +Find(statusId) StatusViewEntry
    }

    class StatusViewEntry{
        <<Serializable>>
        +StatusId : StatusId
        +DisplayName : string
        +Description : string
        +Icon : Sprite
    }

    class StatusPanelPresenter{
        +Notify(StatusAddedEvent) void
        +Notify(StatusRemovedEvent) void
        +Notify(StateRestoredEvent) void
    }

    class StatusPanelViewModel{
        +PlayerId : PlayerId
        +Statuses : IReadOnlyList~StatusState~
    }

    class StatusPanelView{
        +BindViewModel(viewModel) void
        +AddStatusIcon(statusId) void
        +RemoveStatusIcon(statusId) void
    }

    class StatusIconView{
        +BindViewDefinition(entry) void
        +PlayShow() void
        +PlayHide() void
        +ShowDescription() void
        +HideDescription() void
    }

    StatusViewCatalog "1" *-- "0..*" StatusViewEntry
    StatusPanelPresenter --> StatusViewCatalog
    StatusPanelPresenter --> StatusPanelViewModel
    StatusPanelPresenter --> StatusPanelView
    StatusPanelView --> StatusIconView
    StatusIconView --> StatusViewEntry
```

## ReapplyPolicy フロー

```mermaid
sequenceDiagram
    participant Skill as SkillEffectComposer
    participant Applicator as StatusApplicator
    participant Player as PlayerState
    participant Bus as MatchEventBus
    participant Presenter as StatusPanelPresenter

    Skill->>Applicator: Apply(player, newStatus)
    Applicator->>Applicator: StatusDefinition.ReapplyPolicy を確認
    alt Ignore and existing
        Applicator-->>Skill: NoChange
    else Refresh and existing
        Applicator->>Player: RefreshRemaining
        Applicator-->>Skill: StatusAppliedEvent
    else Stack or no existing
        Applicator->>Player: AddStatus
        Applicator-->>Skill: StatusAddedEvent / StatusAppliedEvent
    end
    Skill-->>Bus: IMatchEvent.Dispatch
    Bus-->>Presenter: Notify(event)
```

## ターン開始時の適用フロー

```mermaid
sequenceDiagram
    participant Advancer as TurnAdvancer
    participant Player as PlayerState
    participant Applicator as StatusEffectApplicator
    participant Registry as StatusEffectProcessorRegistry
    participant Processor as IStatusEffectProcessor

    Advancer->>Player: Runtime.Reset / status advance
    Advancer->>Applicator: Apply(player)
    loop each active StatusState
        Applicator->>Registry: Find(effectId)
        Registry-->>Applicator: processor
        Applicator->>Processor: Apply(StatusEffectContext)
        Processor-->>Applicator: events + runtime/skill changes
    end
```

## Status / Effect 一覧

| Enum | 値 |
|---|---|
| `StatusId` | Sleep, Paralysis, SealMovePawn, SealPlaceWall, SealSpecialSkill, RecoveryWall, DamageWall |
| `StatusEffectId` | CannotAct, ProbabilisticCannotAct, CannotMovePawn, CannotPlaceWall, CannotUseSpecialSkill, RecoveryWall, DamageWall |
| `StatusReapplyPolicy` | Ignore, Refresh, Stack |
