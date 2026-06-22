namespace Quoridor
{
    public sealed record StatusAppliedEvent : PlayerTargetEventBase
    {
        public StatusId StatusId { get; }

        public StatusAppliedEvent(PlayerId playerId, StatusId statusId)
            : base(playerId)
        {
            StatusId = statusId;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}