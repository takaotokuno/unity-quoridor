# UnityIntegration / Services

## Unity サービス クラス図

```mermaid
classDiagram
    class GameLifetimeScope{
        <<LifetimeScope>>
    }

    class IGameLogger{
        <<interface>>
        +Log(message) void
        +Warning(message) void
        +Error(message) void
    }

    class UnityGameLogger{
        +Log(message) void
        +Warning(message) void
        +Error(message) void
    }
    IGameLogger <|.. UnityGameLogger

    class ISoundService{
        <<interface>>
        +PlayBgm(id, fadeSeconds) void
        +StopBgm(fadeSeconds) void
        +PlaySe(id) void
        +SetMasterVolume(volume) void
        +SetBgmVolume(volume) void
        +SetSeVolume(volume) void
    }

    class SoundManager{
        <<MonoBehaviour>>
        +PlayBgm(id, fadeSeconds) void
        +StopBgm(fadeSeconds) void
        +PlaySe(id) void
        +SetMasterVolume(volume) void
        +SetBgmVolume(volume) void
        +SetSeVolume(volume) void
    }
    ISoundService <|.. SoundManager

    class SoundCatalog{
        <<ScriptableObject>>
        +FindBgm(id) BgmEntry
        +FindSe(id) SeEntry
    }
    class BgmEntry
    class SeEntry
    SoundManager --> SoundCatalog
    SoundCatalog *-- BgmEntry
    SoundCatalog *-- SeEntry

    class ITimeEffectService{
        <<interface>>
        +ApplyHitStop(duration, timeScale) void
    }
    class TimeEffectManager{
        <<MonoBehaviour>>
        +ApplyHitStop(duration, timeScale) void
    }
    ITimeEffectService <|.. TimeEffectManager

    class IBackgroundEffectService{
        <<interface>>
        +ApplyPreset(presetId) void
        +SetIntensity(value) void
        +Flash(color, duration) void
        +TransitionTo(presetId, duration) void
        +ResetToDefault() void
    }
    class BackgroundEffectManager{
        <<MonoBehaviour>>
        +ApplyPreset(presetId) void
        +SetIntensity(value) void
        +Flash(color, duration) void
        +TransitionTo(presetId, duration) void
        +ResetToDefault() void
    }
    class BackgroundEffectCatalog{
        <<ScriptableObject>>
        +TryGet(presetId, out entry) bool
    }
    class BackgroundEffectEntry
    class BackgroundEffectState{
        <<struct>>
    }
    IBackgroundEffectService <|.. BackgroundEffectManager
    BackgroundEffectManager --> BackgroundEffectCatalog
    BackgroundEffectCatalog *-- BackgroundEffectEntry
    BackgroundEffectManager --> BackgroundEffectState

    class INovelGamePort{
        <<interface>>
        +IsPlaying : bool
        +JumpScenario(scenarioId) void
    }
    class NovelGamePort{
        <<MonoBehaviour>>
        +IsPlaying : bool
        +JumpScenario(scenarioId) void
    }
    class ScenarioCatalog{
        <<ScriptableObject>>
        +FindScenario(id) ScenarioEntry
    }
    class ScenarioEntry
    INovelGamePort <|.. NovelGamePort
    NovelGamePort --> ScenarioCatalog
    ScenarioCatalog *-- ScenarioEntry

    class IRandomProvider{
        <<interface>>
        +Range(min, max) int
        +RollPercent(percent) bool
    }
    class UnityRandomProvider{
        +Range(min, max) int
        +RollPercent(percent) bool
    }
    IRandomProvider <|.. UnityRandomProvider
```

## Board / Novel 連携補助

```mermaid
classDiagram
    class BoardClickInput{
        <<MonoBehaviour>>
    }

    class MatchEventInterpreter{
        +Notify(PawnMovedEvent) void
        +Notify(SkillUsedEvent) void
        +Notify(StatusAppliedEvent) void
        +SubscribeTo(eventBus) void
    }

    MatchEventInterpreter --> ISoundService
    MatchEventInterpreter --> ITimeEffectService
    MatchEventInterpreter --> INovelGamePort
    MatchEventInterpreter --> IBackgroundEffectService
```

## 補足

- 旧ドキュメントの `Instance` ベース Singleton 表記ではなく、現行コードは VContainer の `GameLifetimeScope` とインターフェース注入を中心に組み立てます。
- `SoundManager`、`BackgroundEffectManager`、`TimeEffectManager`、`NovelGamePort` は MonoBehaviour 実装ですが、Application 層からは各インターフェース経由で参照されます。
