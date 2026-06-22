using System;

namespace Quoridor
{
    public sealed class MatchSession : IMatchCommandPort, IDisposable
    {
        public int SessionId { get; }
        private bool _disposed;
        private IMatchCommandPort _commandPort;
        private IMatchEventBus _eventBus;
        private IMatchPresentation _presentation;

        public MatchSession(
            int sessionId,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            IMatchPresentation presentation
        )
        {
            SessionId = sessionId;
            _commandPort = commandPort;
            _eventBus = eventBus;
            _presentation = presentation;
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

            _presentation?.Dispose();
            _presentation = null;           
            _eventBus = null;
            _commandPort = null;

            _disposed = true;
        }
    }
}