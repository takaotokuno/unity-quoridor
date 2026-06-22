namespace Quoridor
{
    public sealed record InteractionStateChangedEvent : MatchEventBase
    {
        public InteractionStateChangedEvent()
        {
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}
