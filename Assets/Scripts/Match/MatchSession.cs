namespace Quoridor
{
    /// <summary>
    /// Facade for interacting with a match. The owning MatchLifetimeScope controls
    /// the lifetime of the objects behind this facade.
    /// </summary>
    public sealed class MatchSession : IMatchCommandPort
    {
        public int SessionId { get; }
        private readonly IMatchCommandPort _commandPort;
        private readonly IMatchEventBus _eventBus;

        public MatchSession(
            int sessionId,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus
        )
        {
            SessionId = sessionId;
            _commandPort = Guard.ThrowIfNull(commandPort, nameof(commandPort));
            _eventBus = Guard.ThrowIfNull(eventBus, nameof(eventBus));
        }

        public IMatchResponse DispatchCommand(IMatchCommand command)
        {
            return _commandPort.DispatchCommand(command);
        }

        public void Subscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
        {
            _eventBus.Subscribe(observer);
        }

        public void Unsubscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
        {
            _eventBus.Unsubscribe(observer);
        }

    }
}
