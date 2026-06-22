namespace Quoridor
{
    public sealed class InteractionStateProjector
        : IMatchObserver<MatchStartedEvent>,
          IMatchObserver<TurnStartedEvent>,
          IMatchObserver<StateRestoredEvent>,
          IMatchObserver<SkillUsedEvent>,
          IMatchObserver<SkillSelectionChangedEvent>,
          IEventSubscriber
    {
        private IMatchEventBus _eventBus;
        private readonly InteractionStateStore _interactionStateStore;

        public InteractionStateProjector(
            InteractionStateStore interactionStateStore
        ) : base()
        {
            _interactionStateStore = interactionStateStore;
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchStartedEvent>(this);
            _eventBus.Subscribe<TurnStartedEvent>(this);
            _eventBus.Subscribe<StateRestoredEvent>(this);
            _eventBus.Subscribe<SkillUsedEvent>(this);
            _eventBus.Subscribe<SkillSelectionChangedEvent>(this);
        }

        public void Notify(MatchStartedEvent e)
        {
            Refresh();
        }

        public void Notify(TurnStartedEvent e)
        {
            Refresh();
        }

        public void Notify(StateRestoredEvent e)
        {
            Refresh();
        }

        public void Notify(SkillUsedEvent e)
        {
            Refresh();
        }

        public void Notify(SkillSelectionChangedEvent e)
        {
            Refresh();
        }

        private void Refresh()
        {
            _interactionStateStore.Refresh();
            _eventBus.DispatchEvent<InteractionStateChangedEvent>(new()); 
        }
    }
}
