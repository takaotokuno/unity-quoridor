namespace Quoridor
{
    public abstract record SkillContextBase
    {
        public MatchState State { get; }
        public PlayerId PlayerId { get; }
        public SkillDefinition Definition { get; }
        public Position? Target { get; }

        protected SkillContextBase(
            MatchState state,
            PlayerId playerId,
            SkillDefinition definition,
            Position? target
        )
        {
            State = state;
            PlayerId = playerId;
            Definition = definition;
            Target = target;
        }
    }
}