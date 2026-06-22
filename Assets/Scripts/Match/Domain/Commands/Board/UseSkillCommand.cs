namespace Quoridor
{
    public sealed record UseSkillCommand : PlayerCommandBase
    {
        public SkillSlotId SkillSlotId { get; }
        public Position? Target { get; }

        public UseSkillCommand(PlayerId playerId, SkillSlotId skillSlotId, Position? target, string issuer) 
            : base(playerId, issuer)
        {
            SkillSlotId = skillSlotId;
            Target = target;
        }

        public override CommandResult Execute(ICommandVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}