namespace Quoridor
{
    public sealed record StateRestoredEvent : MatchEventBase
    {
        public MatchMemento State { get; }

        public StateRestoredEvent(MatchMemento state)
            : base()
        {
            State = Guard.ThrowIfNull(state, nameof(state));
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}
