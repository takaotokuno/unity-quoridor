namespace Quoridor
{
    public sealed record InputReceivedEvent : MatchEventBase
    {
        public InputTarget Target { get; }
        public InputIntent Intent { get; } 

        public InputReceivedEvent(InputTarget target, InputIntent intent)
        {
            Target = target;
            Intent = intent;   
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}