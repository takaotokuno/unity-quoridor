namespace Quoridor
{
    public abstract record PlayerTargetEventBase : MatchEventBase
    {
        public PlayerId PlayerId { get; }
        protected PlayerTargetEventBase(PlayerId playerId)
        {
            PlayerId = playerId;   
        }   
    }
}