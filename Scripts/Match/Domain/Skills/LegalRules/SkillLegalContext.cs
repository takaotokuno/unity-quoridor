namespace Quoridor
{
    public sealed record SkillLegalContext : SkillContextBase
    {
        public SkillLegalContext(
            MatchState state,
            PlayerId playerId,
            SkillDefinition definition,
            Position? target
        ) : base(state, playerId, definition, target)
        {
        }
    }
}