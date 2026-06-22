namespace Quoridor
{
    public sealed record TurnSkippedEvent : PlayerTargetEventBase
    {
        public int CurrentTurn { get; }
        public TurnSkippedEvent(PlayerId playerId, int currentTurn)
            : base(playerId)
        {
            CurrentTurn = currentTurn;   
        }   

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}