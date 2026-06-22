namespace Quoridor
{
    public sealed record CommandRejectedEvent : MatchEventBase 
    {
        public IMatchCommand Command { get; }
        public string Message { get; }

        public CommandRejectedEvent(IMatchCommand command, string message)
            : base()
        {
            Command = command;
            Message = message;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}