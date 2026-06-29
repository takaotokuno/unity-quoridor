namespace Quoridor
{
    public sealed class SkillButtonPresenter 
        : PresenterBase,
          IMatchObserver<InteractionStateChangedEvent>,
          IMatchObserver<InputReceivedEvent>,
          IEventSubscriber
    {
        private readonly PlayerSkillButtonPresenter _firstPlayer;
        private readonly PlayerSkillButtonPresenter _secondPlayer;

        private IMatchEventBus _eventBus;

        public SkillButtonPresenter(
            SkillButtonViewModel[] buttonModelsFirst,
            SkillButtonViewModel[] buttonModelsSecond,
            InteractionStateStore interactionStateStore,
            InputStateStore inputStateStore,
            SkillSelectionStore skillSelectionStore
        ) : base()
        {
            _firstPlayer = new PlayerSkillButtonPresenter(
                PlayerId.FirstPlayer,
                buttonModelsFirst,
                interactionStateStore,
                inputStateStore,
                skillSelectionStore
            );

            _secondPlayer = new PlayerSkillButtonPresenter(
                PlayerId.SecondPlayer,
                buttonModelsSecond,
                interactionStateStore,
                inputStateStore,
                skillSelectionStore
            );
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;

            _eventBus.Subscribe<InteractionStateChangedEvent>(this);
            _eventBus.Subscribe<InputReceivedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<InteractionStateChangedEvent>(this);
            _eventBus.Unsubscribe<InputReceivedEvent>(this);
            _eventBus = null;
        }

        public void Notify(InteractionStateChangedEvent e)
        {
            Refresh();
        }

        public void Notify(InputReceivedEvent e)
        {
            Refresh();
        }

        private void Refresh()
        {
            _firstPlayer.Refresh();
            _secondPlayer.Refresh();
        }
    }
}
