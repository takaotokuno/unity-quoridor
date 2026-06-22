namespace Quoridor
{
    public sealed record MatchStartedEvent : PlayerTargetEventBase
    {
        public MatchStartedEvent(PlayerId playerId)
            : base(playerId)
        {
            
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}