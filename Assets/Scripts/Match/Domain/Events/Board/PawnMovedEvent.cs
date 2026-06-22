namespace Quoridor
{
    public sealed record PawnMovedEvent : BoardTargetEventBase
    {
        public PlayerId PlayerId { get; }

        public PawnMovedEvent(PlayerId playerId, Position target)
            : base(target)
        {
            PlayerId = playerId;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}