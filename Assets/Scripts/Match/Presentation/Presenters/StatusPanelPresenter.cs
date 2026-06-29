namespace Quoridor
{
    public sealed class StatusPanelPresenter
        : PresenterBase,
          IMatchObserver<InteractionStateChangedEvent>,
          IMatchObserver<DistanceUpdatedEvent>,
          IMatchObserver<StatusAddedEvent>,
          IMatchObserver<StatusRemovedEvent>,
          IEventSubscriber
    {
        private readonly PlayerStatusPanelPresenter _firstPlayer;
        private readonly PlayerStatusPanelPresenter _secondPlayer;

        private IMatchEventBus _eventBus;

        public StatusPanelPresenter(
            PlayerStatusPanelPresenter firstPlayer,
            PlayerStatusPanelPresenter secondPlayer
        ) : base()
        {
            _firstPlayer = Guard.ThrowIfNull(
                firstPlayer,
                nameof(firstPlayer)
            );
            _secondPlayer = Guard.ThrowIfNull(
                secondPlayer,
                nameof(secondPlayer)
            );
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<InteractionStateChangedEvent>(this);
            _eventBus.Subscribe<DistanceUpdatedEvent>(this);
            _eventBus.Subscribe<StatusAddedEvent>(this);
            _eventBus.Subscribe<StatusRemovedEvent>(this);
        }

        public override void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<InteractionStateChangedEvent>(this);
            _eventBus.Unsubscribe<DistanceUpdatedEvent>(this);
            _eventBus.Unsubscribe<StatusAddedEvent>(this);
            _eventBus.Unsubscribe<StatusRemovedEvent>(this);
            _eventBus = null;
        }

        public void Notify(InteractionStateChangedEvent e)
        {
            RefreshRemainWallCount();
        }

        public void Notify(DistanceUpdatedEvent e)
        {
            UpdateDistance(e.Distances);
        }

        public void Notify(StatusAddedEvent e)
        {
            GetPlayerPresenter(e.PlayerId).AddStatusIcon(e.StatusId);
        }

        public void Notify(StatusRemovedEvent e)
        {
            // イベントが来た時点で重ねがけが全て解消されたことが保証されている
            GetPlayerPresenter(e.PlayerId).RemoveStatusIcon(e.StatusId);
        }

        private void RefreshRemainWallCount()
        {
            _firstPlayer.RefreshRemainWallCount();
            _secondPlayer.RefreshRemainWallCount();
        }

        private void UpdateDistance(DistanceSnapshot distances)
        {
            _firstPlayer.UpdateDistance(distances);
            _secondPlayer.UpdateDistance(distances);
        }

        private PlayerStatusPanelPresenter GetPlayerPresenter(PlayerId playerId)
        {
            return playerId.IsFirstPlayer
                ? _firstPlayer
                : _secondPlayer;
        }
    }
}
