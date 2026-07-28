using System;
using System.Collections.Generic;
using System.Threading;

namespace Quoridor
{
    public sealed class MatchCommandPort : IMatchCommandPort, IDisposable
    {
        private readonly Queue<IMatchCommand> _queue = new();
        private readonly object _lock = new();
        private readonly MatchCommandExecutor _executor;
        private readonly SynchronizationContext _mainThreadContext;
        private readonly IGameLogger _logger;
        private readonly MatchLifetimeState _lifetime;

        private bool _isProcessing;
        private bool _isProcessPosted;
        private bool _disposed;

        public MatchCommandPort(
            MatchCommandExecutor executor,
            SynchronizationContext mainThreadContext,
            IGameLogger logger,
            MatchLifetimeState lifetime
        )
        {
            _executor = Guard.ThrowIfNull(executor, nameof(executor));
            _mainThreadContext = Guard.ThrowIfNull(mainThreadContext, nameof(mainThreadContext));
            _logger = logger;
            _lifetime = Guard.ThrowIfNull(lifetime, nameof(lifetime));
        }

        public IMatchResponse DispatchCommand(IMatchCommand command)
        {
            _lifetime.ThrowIfDisposed();
            LogCommand(command);

            if (command == null)
            {
                return new CommandRejectedResponse("Command is null");
            }

            lock (_lock)
            {
                _lifetime.ThrowIfDisposed();
                ThrowIfDisposed();
                _queue.Enqueue(command);

                if (!_isProcessing && !_isProcessPosted)
                {
                    _isProcessPosted = true;
                    _mainThreadContext.Post(_ => ProcessQueue(), null);
                }
            }

            return new CommandAcceptedResponse();
        }

        private void ProcessQueue()
        {
            lock (_lock)
            {
                _isProcessPosted = false;

                if (_disposed || _lifetime.IsDisposed || _isProcessing)
                {
                    return;
                }

                _isProcessing = true;
            }

            try
            {
                while (true)
                {
                    IMatchCommand command;

                    lock (_lock)
                    {
                        if (_disposed || _lifetime.IsDisposed || _queue.Count == 0)
                        {
                            return;
                        }

                        command = _queue.Dequeue();
                    }

                    ExecuteSafely(command);
                }
            }
            finally
            {
                lock (_lock)
                {
                    _isProcessing = false;

                    if (!_disposed && !_lifetime.IsDisposed &&
                        _queue.Count > 0 && !_isProcessPosted)
                    {
                        _isProcessPosted = true;
                        _mainThreadContext.Post(_ => ProcessQueue(), null);
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed) return;

                _disposed = true;
                _queue.Clear();
                _isProcessPosted = false;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MatchCommandPort));
            }
        }

        private void ExecuteSafely(IMatchCommand command)
        {
            try
            {
                _executor.Execute(command);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        private void LogCommand<T>(T c)
        {
            _logger.Log(LogFormatter.Format("Command", c));
        }
    }
}
