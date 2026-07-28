namespace Quoridor
{
    /// <summary>
    /// Facade for interacting with a match. The owning MatchLifetimeScope controls
    /// the lifetime of the objects behind this facade.
    /// </summary>
    public sealed class MatchSession : IMatchCommandPort
    {
        public MatchSessionId SessionId { get; }
        private readonly IMatchCommandPort _commandPort;
        private readonly IMatchEventBus _eventBus;
        private readonly MatchLifetimeState _lifetime;

        public MatchSession(
            MatchSessionId sessionId,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            MatchLifetimeState lifetime
        )
        {
            SessionId = Guard.ThrowIfNull(sessionId, nameof(sessionId));
            _commandPort = Guard.ThrowIfNull(commandPort, nameof(commandPort));
            _eventBus = Guard.ThrowIfNull(eventBus, nameof(eventBus));
            _lifetime = Guard.ThrowIfNull(lifetime, nameof(lifetime));
        }

        public IMatchResponse DispatchCommand(IMatchCommand command)
        {
            _lifetime.ThrowIfDisposed();
            return _commandPort.DispatchCommand(command);
        }

        public void Subscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
        {
            _lifetime.ThrowIfDisposed();
            _eventBus.Subscribe(observer);
        }

        public void Unsubscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
        {
            _lifetime.ThrowIfDisposed();
            _eventBus.Unsubscribe(observer);
        }

    }
}
