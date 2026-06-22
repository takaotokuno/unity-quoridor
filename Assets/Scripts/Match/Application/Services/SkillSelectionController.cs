namespace Quoridor
{
    public sealed class SkillSelectionController
    {
        private readonly SkillSelectionStore _skillSelectionStore;
        private readonly IMatchEventBus _eventBus;

        public SkillSelectionController(
            SkillSelectionStore skillSelectionStore,
            IMatchEventBus eventBus
        )
        {
            _skillSelectionStore = skillSelectionStore;
            _eventBus = eventBus;
        }

        public SkillSlotId SelectedSkillSlotId => _skillSelectionStore.SelectedSkillSlotId;

        public void Clear(PlayerId playerId)
        {
            _skillSelectionStore.Clear();
            DispatchChanged(playerId, null);
        }

        public void Toggle(InputTarget target)
        {
            if (IsSelected(target))
            {
                Clear(target.PlayerId);
                return;
            }

            _skillSelectionStore.Select(target);
            DispatchChanged(target.PlayerId, target.SkillSlotId);
        }

        private bool IsSelected(InputTarget target)
        {
            return _skillSelectionStore.SelectedPlayerId == target.PlayerId
                   && _skillSelectionStore.SelectedSkillSlotId == target.SkillSlotId;
        }

        private void DispatchChanged(PlayerId playerId, SkillSlotId skillSlotId)
        {
            _eventBus.DispatchEvent<SkillSelectionChangedEvent>(
                new(playerId, skillSlotId)
            );
        }
    }
}
