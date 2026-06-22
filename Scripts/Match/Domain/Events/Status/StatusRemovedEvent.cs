namespace Quoridor
{
    public sealed record StatusRemovedEvent : PlayerTargetEventBase
    {
        public StatusId StatusId { get; }
        public StatusRemovedEvent(PlayerId playerId, StatusId statusId)
            : base(playerId)
        {
            StatusId = statusId;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}