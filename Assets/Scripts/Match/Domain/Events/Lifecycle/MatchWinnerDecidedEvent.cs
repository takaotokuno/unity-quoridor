namespace Quoridor
{
    public sealed record MatchWinnerDecidedEvent : PlayerTargetEventBase
    {
        public MatchWinnerDecidedEvent(PlayerId playerId)
            : base(playerId)
        {
            
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}