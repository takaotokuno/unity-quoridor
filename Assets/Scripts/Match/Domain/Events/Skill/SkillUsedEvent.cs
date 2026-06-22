namespace Quoridor
{
    public sealed record SkillUsedEvent : PlayerTargetEventBase
    {
        public SkillId SkillId { get; }
        public SkillSlotId SkillSlotId { get; }

        public SkillUsedEvent(PlayerId playerId, SkillId skillId, SkillSlotId skillSlotId)
            : base(playerId)
        {
            SkillId = skillId;
            SkillSlotId = skillSlotId;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}