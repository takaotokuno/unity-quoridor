namespace Quoridor
{
    public sealed class BoardPawnPresenter
        : PresenterBase,
          IMatchObserver<PawnMovedEvent>,
          IEventSubscriber
    {
        private readonly PawnViewModel[] _viewModels;
        private IMatchEventBus _eventBus;

        public BoardPawnPresenter(PawnViewModel[] viewModels)
            : base()
        {
            _viewModels = Guard.ThrowIfNull(viewModels, nameof(viewModels));
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<PawnMovedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<PawnMovedEvent>(this);
            _eventBus = null;
        }

        public void Notify(PawnMovedEvent e)
        {
            _viewModels[e.PlayerId.ToIndex()].Position = e.Target;
        }
    }
}
