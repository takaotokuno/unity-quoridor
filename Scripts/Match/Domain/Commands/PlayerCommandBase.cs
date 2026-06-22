namespace Quoridor
{
    public abstract record PlayerCommandBase : MatchCommandBase
    {
        public PlayerId PlayerId { get; }

        protected PlayerCommandBase(PlayerId playerId, string issuer) 
            : base(issuer)
        {
            PlayerId = playerId;
        }
    }
}