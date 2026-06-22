namespace Quoridor
{
    public sealed record InputRejectedEvent : MatchEventBase
    {
        public InputTarget Target { get; }
        public InputIntent Intent { get; }
        public string Reason { get; }

        public InputRejectedEvent(InputTarget target, InputIntent intent, string reason)
        {
            Target = target;
            Intent = intent;
            Reason = reason;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}
