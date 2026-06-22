namespace Quoridor
{
    public sealed record TurnStartedEvent : PlayerTargetEventBase
    {
        public int CurrentTurn { get; }
        public TurnStartedEvent(PlayerId playerId, int currentTurn)
            : base(playerId)
        {
            CurrentTurn = currentTurn;   
        }   

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}