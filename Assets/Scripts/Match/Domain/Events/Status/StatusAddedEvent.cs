namespace Quoridor
{
    public sealed record StatusAddedEvent : PlayerTargetEventBase
    {
        public StatusId StatusId { get; }

        public StatusAddedEvent(PlayerId playerId, StatusId statusId)
            : base(playerId)
        {
            StatusId = statusId;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}