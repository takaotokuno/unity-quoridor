namespace Quoridor
{
    public sealed record SkillSelectionChangedEvent : PlayerTargetEventBase
    {
        public SkillSlotId SkillSlotId { get; }

        public SkillSelectionChangedEvent(PlayerId playerId, SkillSlotId skillSlotId)
            : base(playerId)
        {
            SkillSlotId = skillSlotId;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}