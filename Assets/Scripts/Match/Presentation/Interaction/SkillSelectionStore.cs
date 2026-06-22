namespace Quoridor
{
    public sealed class SkillSelectionStore
    {
        public PlayerId SelectedPlayerId { get; private set; }
        public SkillSlotId SelectedSkillSlotId { get; private set; }

        public void Select(InputTarget target)
        {
            if(target.Kind != InputTargetKind.SkillButton) return;
            SelectedPlayerId = target.PlayerId;
            SelectedSkillSlotId = target.SkillSlotId;
        }

        public void Clear()
        {
            SelectedPlayerId = null;
            SelectedSkillSlotId = null;
        }
    }
}
