using System;

namespace Quoridor
{
    public sealed class MatchSession : IMatchCommandPort, IDisposable
    {
        public int SessionId { get; }
        private bool _disposed;
        private IMatchCommandPort _commandPort;
        private IMatchEventBus _eventBus;
        private IMatchObjects _matchObjects;

        public MatchSession(
            int sessionId,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            IMatchObjects matchObjects
        )
        {
            SessionId = sessionId;
            _commandPort = commandPort;
            _eventBus = eventBus;
            _matchObjects = matchObjects;
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

        public void Dispose()
        {
            if (_disposed) return;

            _matchObjects?.Dispose();
            _matchObjects = null;
            _eventBus = null;
            _commandPort = null;

            _disposed = true;
        }
    }
}
