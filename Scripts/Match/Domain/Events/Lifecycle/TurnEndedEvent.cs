namespace Quoridor
{
    public sealed record TurnEndedEvent : PlayerTargetEventBase
    {
        public TurnEndedEvent(PlayerId playerId)
            : base(playerId)
        {
            
        }   

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}