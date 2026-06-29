namespace Quoridor
{
    public sealed class PlayerStatusPanelPresenter
    {
        private readonly PlayerId _playerId;
        private readonly StatusPanelViewModel _panelViewModel;
        private readonly IStatusIconContainer _iconContainer;
        private readonly InteractionStateStore _interactionStateStore;

        public PlayerStatusPanelPresenter(
            PlayerId playerId,
            StatusPanelViewModel panelViewModel,
            IStatusIconContainer iconContainer,
            InteractionStateStore interactionStateStore
        )
        {
            _playerId = playerId;
            _panelViewModel = Guard.ThrowIfNull(
                panelViewModel,
                nameof(panelViewModel)
            );
            _iconContainer = Guard.ThrowIfNull(
                iconContainer,
                nameof(iconContainer)
            );
            _interactionStateStore = Guard.ThrowIfNull(
                interactionStateStore,
                nameof(interactionStateStore)
            );
        }

        public void RefreshRemainWallCount()
        {
            InteractionState skillState = _interactionStateStore.GetSkillState(
                _playerId,
                BuiltInSkillSlotIds.PlaceWall
            );

            _panelViewModel.RemainWallCount = skillState.RemainingUses;
        }

        public void UpdateDistance(DistanceSnapshot distances)
        {
            _panelViewModel.Distance = distances.GetDistance(_playerId);
        }

        public void AddStatusIcon(StatusId statusId)
        {
            _iconContainer.AddStatusIcon(statusId);
        }

        public void RemoveStatusIcon(StatusId statusId)
        {
            _iconContainer.RemoveStatusIcon(statusId);
        }
    }
}
