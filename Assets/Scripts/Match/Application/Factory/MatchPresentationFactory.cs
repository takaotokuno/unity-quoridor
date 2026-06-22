namespace Quoridor
{
    public sealed class MatchPresentationFactory
    {
        private readonly ISkillDefinitionRegistry _skillDefinitionRegistry;
        private readonly SkillViewCatalog _skillViewCatalog;
        private readonly StatusViewCatalog _statusViewCatalog;
        private readonly InteractionStateCalculator _stateCalculator;

        public MatchPresentationFactory(
            ISkillDefinitionRegistry skillDefinitionRegistry,
            SkillViewCatalog skillViewCatalog,
            StatusViewCatalog statusViewCatalog,
            InteractionStateCalculator stateCalculator
        )
        {
            _skillDefinitionRegistry = skillDefinitionRegistry;
            _skillViewCatalog = skillViewCatalog;
            _statusViewCatalog = statusViewCatalog;
            _stateCalculator = stateCalculator;
        }

        public MatchPresentation Create(
            MatchPresentationConfig config, 
            MatchState state,
            IMatchEventBus eventBus,
            IMatchCommandPort commandPort
        )
        {
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

            return new MatchPresentation(
                config.ObjectLayoutView,
                control,
                board,
                turn,
                skill,
                status
            );
        }
    }
}