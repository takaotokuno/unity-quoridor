using System;
using System.Collections.Generic;
using System.Threading;
using Quoridor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class GameLifetimeScope : LifetimeScope
{
    [Header("Unity Scene Components")]
    [SerializeField] private BackgroundEffectManager backgroundEffectManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private TimeEffectManager timeEffectManager;

    [Header("Skill Catalogs")]
    [SerializeField] private SkillCatalog skillCatalog;
    [SerializeField] private SkillViewCatalog skillViewCatalog;

    [Header("Status Catalogs")]
    [SerializeField] private StatusCatalog statusCatalog;
    [SerializeField] private StatusViewCatalog statusViewCatalog;

    [Header("Other")]
    [SerializeField] private DebugPort debugPort;

    protected override void Configure(IContainerBuilder builder)
    {
        RegisterMainThreadContext(builder);
        RegisterUnityComponents(builder);
        RegisterSharedRulesAndValidators(builder);
        RegisterSkillSystem(builder);
        RegisterStatusSystem(builder);
        RegisterFactories(builder);
        RegisterEntryPoints(builder);

        builder.RegisterComponent<DebugPort>(debugPort);
        builder.Register<IGameLogger, UnityGameLogger>(Lifetime.Singleton);
    }

    private void RegisterMainThreadContext(IContainerBuilder builder)
    {
        var context = SynchronizationContext.Current;

        if (context == null)
        {
            throw new InvalidOperationException(
                "SynchronizationContext.Current is null. GameLifetimeScope must be configured on Unity main thread."
            );
        }

        builder.RegisterInstance(context);
    }

    /// <summary>
    /// Scene上に配置済みのUnityコンポーネントをDIへ登録する。
    /// MonoBehaviour由来のものは RegisterComponent / RegisterInstance を使う。
    /// </summary>
    private void RegisterUnityComponents(IContainerBuilder builder)
    {
        builder.RegisterComponent<IBackgroundEffectService>(backgroundEffectManager);
        builder.RegisterComponent<ISoundService>(soundManager);
        builder.RegisterComponent<ITimeEffectService>(timeEffectManager);

        builder.RegisterInstance(skillCatalog);
        builder.RegisterInstance(skillViewCatalog);

        builder.RegisterInstance(statusCatalog);
        builder.RegisterInstance(statusViewCatalog);
    }

    /// <summary>
    /// MatchState、CommandPort、Presentationなどを生成するFactory群。
    /// 状態を直接持たないため Singleton でよい。
    /// </summary>
    private void RegisterFactories(IContainerBuilder builder)
    {
        builder.Register<MatchFactory>(Lifetime.Singleton);
        builder.Register<MatchStateFactory>(Lifetime.Singleton);
        builder.Register<CommandHandlerFactory>(Lifetime.Singleton);
        builder.Register<MatchCommandExecutorFactory>(Lifetime.Singleton);
        builder.Register<MatchCommandPortFactory>(Lifetime.Singleton);
        builder.Register<MatchPresentationFactory>(Lifetime.Singleton);
        builder.Register<LegalCommandEnumerator>(Lifetime.Singleton);
        builder.Register<CpuAgentStrategyFactory>(Lifetime.Singleton);
        builder.Register<CpuAgentFactory>(Lifetime.Singleton);
    }

    /// <summary>
    /// 盤面・移動・勝敗判定など、複数ルールから使われる共通ロジック。
    /// MatchStateを保持せず、引数で受け取る設計なら Singleton でよい。
    /// </summary>
    private void RegisterSharedRulesAndValidators(IContainerBuilder builder)
    {
        builder.Register<GoalResolver>(Lifetime.Singleton);
        builder.Register<CheckmateResolver>(Lifetime.Singleton);
        builder.Register<SkillAvailabilityValidator>(Lifetime.Singleton);
        builder.Register<Pathfinder>(Lifetime.Singleton);
        builder.Register<DistanceCalculator>(Lifetime.Singleton);

        builder.Register<WallPlacementPatternProvider>(Lifetime.Singleton);
        builder.Register<ExistingWallPatternResolver>(Lifetime.Singleton);

        builder.Register<MovePawnValidator>(Lifetime.Singleton);
        builder.Register<PlaceWallValidator>(Lifetime.Singleton);

        builder.Register<MatchResultResolver>(Lifetime.Singleton);
        builder.Register<MatchEventInterpreter>(Lifetime.Singleton);

        builder.Register<StatusApplicator>(Lifetime.Singleton);
        builder.Register<StatusEffectApplicator>(Lifetime.Singleton);
        builder.Register<TurnAdvancer>(Lifetime.Singleton);

        builder.Register<IRandomProvider, UnityRandomProvider>(Lifetime.Singleton);
    }
    /// <summary>
    /// SkillDefinition、SkillEffect、SkillLegalRule周り。
    /// CatalogはUnity上のScriptableObject等を元にし、Registryへ変換して使う。
    /// </summary>
    private void RegisterSkillSystem(IContainerBuilder builder)
    {
        builder.Register<ISkillDefinitionRegistry>(container =>
        {
            SkillCatalog catalog = container.Resolve<SkillCatalog>();

            IReadOnlyList<SkillDefinition> definitions =
                SkillCatalogConverter.Convert(catalog);

            return new SkillDefinitionRegistry(definitions);
        }, Lifetime.Singleton);

        // Skill effect composers
        builder.Register<ISkillEffectComposer, MovePawnEffectComposer>(Lifetime.Singleton);
        builder.Register<ISkillEffectComposer, PlaceWallEffectComposer>(Lifetime.Singleton);
        builder.Register<ISkillEffectComposer, RemoveWallEffectComposer>(Lifetime.Singleton);
        builder.Register<ISkillEffectComposer, ApplyStatusEffectComposer>(Lifetime.Singleton);

        builder.Register<ISkillEffectComposerRegistry, SkillEffectComposerRegistry>(Lifetime.Singleton);

        // Skill legal rules
        builder.Register<ISkillLegalRule, MovePawnLegalRule>(Lifetime.Singleton);
        builder.Register<ISkillLegalRule, PlaceWallLegalRule>(Lifetime.Singleton);
        builder.Register<ISkillLegalRule, RemoveWallLegalRule>(Lifetime.Singleton);
        builder.Register<ISkillLegalRule, NoneLegalRule>(Lifetime.Singleton);
        builder.Register<ISkillLegalRule, AnyTileLegalRule>(Lifetime.Singleton);
        builder.Register<ISkillLegalRule, AnyWallLegalRule>(Lifetime.Singleton);

        builder.Register<ISkillLegalRuleRegistry, SkillLegalRuleRegistry>(Lifetime.Singleton);

        builder.Register<InteractionStateCalculator>(Lifetime.Singleton);
    }

    /// <summary>
    /// StatusDefinition、StatusEffect周り。
    /// CatalogはUnity上のScriptableObject等を元にし、Registryへ変換して使う。
    /// </summary>
    private void RegisterStatusSystem(IContainerBuilder builder)
    {
        builder.Register<IStatusDefinitionRegistry>(container =>
        {
            StatusCatalog catalog = container.Resolve<StatusCatalog>();

            IReadOnlyList<StatusDefinition> definitions =
                StatusCatalogConverter.Convert(catalog);

            return new StatusDefinitionRegistry(definitions);
        }, Lifetime.Singleton);

        // Status effect composers
        builder.Register<IStatusEffectProcessor, CannotActStatusEffectProcessor>(Lifetime.Singleton);
        builder.Register<IStatusEffectProcessor, ProbabilisticCannotActStatusEffectProcessor>(Lifetime.Singleton);
        builder.Register<IStatusEffectProcessor, CannotMovePawnStatusEffectProcessor>(Lifetime.Singleton);
        builder.Register<IStatusEffectProcessor, CannotPlaceWallStatusEffectProcessor>(Lifetime.Singleton);
        builder.Register<IStatusEffectProcessor, CannotUseSpecialSkillStatusEffectProcessor>(Lifetime.Singleton);
        builder.Register<IStatusEffectProcessor, RecoveryWallStatusEffectProcessor>(Lifetime.Singleton);
        builder.Register<IStatusEffectProcessor, DamageWallStatusEffectProcessor>(Lifetime.Singleton);

        builder.Register<IStatusEffectProcessorRegistry, StatusEffectProcessorRegistry>(Lifetime.Singleton);
    }

    /// <summary>
    /// ゲーム外部との窓口・起動点。
    /// BoardGamePortはノベルゲーム側など外部から呼ばれる窓口。
    /// GameDirectorはゲーム全体の進行管理。
    /// </summary>
    private void RegisterEntryPoints(IContainerBuilder builder)
    {
        builder.Register<GameDirector>(Lifetime.Singleton);
        builder.Register<BoardGamePort>(Lifetime.Singleton);
    }
}
