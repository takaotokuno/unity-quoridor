namespace Quoridor
{
    public sealed class MatchObjectsFactory
    {
        private readonly ISkillDefinitionRegistry _skillDefinitionRegistry;
        private readonly CpuAgentFactory _cpuAgentFactory;
        private readonly SkillViewCatalog _skillViewCatalog;
        private readonly StatusViewCatalog _statusViewCatalog;
        private readonly InteractionStateCalculator _stateCalculator;

        public MatchObjectsFactory(
            ISkillDefinitionRegistry skillDefinitionRegistry,
            CpuAgentFactory cpuAgentFactory,
            SkillViewCatalog skillViewCatalog,
            StatusViewCatalog statusViewCatalog,
            InteractionStateCalculator stateCalculator
        )
        {
            _skillDefinitionRegistry = Guard.ThrowIfNull(skillDefinitionRegistry, nameof(skillDefinitionRegistry));
            _cpuAgentFactory = Guard.ThrowIfNull(cpuAgentFactory, nameof(cpuAgentFactory));
            _skillViewCatalog = Guard.ThrowIfNull(skillViewCatalog, nameof(skillViewCatalog));
            _statusViewCatalog = Guard.ThrowIfNull(statusViewCatalog, nameof(statusViewCatalog));
            _stateCalculator = Guard.ThrowIfNull(stateCalculator, nameof(stateCalculator));
        }

        public MatchObjects Create(
            MatchObjectsConfig config,
            MatchStateConfig stateConfig,
            MatchState state,
            IMatchEventBus eventBus,
            IMatchCommandPort commandPort
        )
        {
            ValidateCreateArguments(
                config,
                stateConfig,
                state,
                eventBus,
                commandPort
            );

            var cpuAgents = _cpuAgentFactory.Create(
                stateConfig,
                state,
                commandPort,
                eventBus,
                config.ObjectLayoutView.transform
            );

            SkillSelectionStore skillSelectionStore = new();
            InteractionStateStore interactionStateStore = new(
                state,
                _stateCalculator,
                skillSelectionStore
            );
            InputStateStore inputStateStore = new();

            InteractionStateProjector interactionStateProjector = new(interactionStateStore);
            interactionStateProjector.SubscribeTo(eventBus);

            MatchInputStateUpdater inputStateUpdater = new(inputStateStore, eventBus);
            MatchInputRejectionDispatcher inputRejectionDispatcher = new(eventBus);
            MatchInputReleaseValidator inputReleaseValidator = new(
                interactionStateStore,
                inputRejectionDispatcher
            );
            SkillSelectionController skillSelectionController = new(
                skillSelectionStore,
                eventBus
            );
            MatchInputCommandDispatcher inputCommandDispatcher = new(
                state,
                commandPort,
                _skillDefinitionRegistry,
                skillSelectionController,
                inputRejectionDispatcher
            );
            MatchInputPort inputPort = new(
                inputStateUpdater,
                inputReleaseValidator,
                inputCommandDispatcher
            );

            IPresenterFactory factory = new PresenterFactory(
                config,
                inputPort,
                eventBus,
                interactionStateStore,
                inputStateStore,
                skillSelectionStore,
                _skillViewCatalog,
                _statusViewCatalog
            );

            var control = factory.CreateControl();
            var board = factory.CreateBoard();
            var turn = factory.CreateTurnPanel();
            var skill = factory.CreateSkillButton();
            var status = factory.CreateStatusPanel();

            return new MatchObjects(
                config.ObjectLayoutView,
                cpuAgents,
                control,
                board,
                turn,
                skill,
                status
            );
        }

        private static void ValidateCreateArguments(
            MatchObjectsConfig config,
            MatchStateConfig stateConfig,
            MatchState state,
            IMatchEventBus eventBus,
            IMatchCommandPort commandPort
        )
        {
            Guard.ThrowIfNull(config, nameof(config));
            Guard.ThrowIfNull(stateConfig, nameof(stateConfig));
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(eventBus, nameof(eventBus));
            Guard.ThrowIfNull(commandPort, nameof(commandPort));
        }
    }
}
