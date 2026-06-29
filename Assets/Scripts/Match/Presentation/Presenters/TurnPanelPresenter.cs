namespace Quoridor
{
    public sealed class TurnPanelPresenter 
        : PresenterBase,
          IMatchObserver<MatchReadiedEvent>, 
          IMatchObserver<TurnStartedEvent>,
          IEventSubscriber
    {
        private readonly TurnPanelViewModel _viewModel;
        private IMatchEventBus _eventBus;

        public TurnPanelPresenter(TurnPanelViewModel viewModel)
            : base()
        {
            _viewModel = viewModel;
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchReadiedEvent>(this);
            _eventBus.Subscribe<TurnStartedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<MatchReadiedEvent>(this);
            _eventBus.Unsubscribe<TurnStartedEvent>(this);
            _eventBus = null;
        }

        public void Notify(MatchReadiedEvent e)
        {
            _viewModel.IsVisible = true;
        }

        public void Notify(TurnStartedEvent e)
        {
            _viewModel.CurrentTurn = e.CurrentTurn;
        }
    }
}
