namespace Quoridor
{
    public sealed record SkillEffectContext : SkillContextBase
    {
        public SkillEffectContext(
            MatchState state,
            PlayerId playerId,
            SkillDefinition definition,
            Position? target
        ) : base(state, playerId, definition, target)
        {
        }
    }
}
