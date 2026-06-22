using System;
using System.Collections.Generic;
using System.Threading;

namespace Quoridor
{
    public sealed class MatchCommandPort : IMatchCommandPort
    {
        private readonly Queue<IMatchCommand> _queue = new();
        private readonly object _lock = new();
        private readonly MatchCommandExecutor _executor;
        private readonly SynchronizationContext _mainThreadContext;
        private readonly IGameLogger _logger;

        private bool _isProcessing;
        private bool _isProcessPosted;

        public MatchCommandPort(
            MatchCommandExecutor executor,
            SynchronizationContext mainThreadContext,
            IGameLogger logger
        )
        {
            _executor = Guard.ThrowIfNull(executor, nameof(executor));
            _mainThreadContext = Guard.ThrowIfNull(mainThreadContext, nameof(mainThreadContext));
            _logger = logger;
        }

        public IMatchResponse DispatchCommand(IMatchCommand command)
        {
            LogCommand(command);

            if (command == null)
            {
                return new CommandRejectedResponse("Command is null");
            }

            lock (_lock)
            {
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

                if (_isProcessing)
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
                        if (_queue.Count == 0)
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

                    if (_queue.Count > 0 && !_isProcessPosted)
                    {
                        _isProcessPosted = true;
                        _mainThreadContext.Post(_ => ProcessQueue(), null);
                    }
                }
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