namespace Quoridor
{
    public sealed record CheckmateEvent : PlayerTargetEventBase 
    {
        public CheckmateEvent(PlayerId playerId)
            : base(playerId)
        {
            
        }   

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}