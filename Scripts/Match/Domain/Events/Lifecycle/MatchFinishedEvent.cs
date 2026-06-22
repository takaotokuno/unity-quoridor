namespace Quoridor
{
    public sealed record MatchFinishedEvent : MatchEventBase 
    {
        public MatchFinishedEvent() : base() {}

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}