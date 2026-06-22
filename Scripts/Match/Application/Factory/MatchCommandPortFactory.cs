using System.Threading;

namespace Quoridor
{
    public sealed class MatchCommandPortFactory
    {
        private readonly CommandHandlerFactory _commandHandlerFactory;
        private readonly MatchCommandExecutorFactory _executorFactory;
        private readonly SynchronizationContext _mainThreadContext;
        private readonly IGameLogger _logger;

        public MatchCommandPortFactory(
            CommandHandlerFactory commandHandlerFactory,
            MatchCommandExecutorFactory executorFactory,
            SynchronizationContext mainThreadContext,
            IGameLogger logger
        )
        {
            _commandHandlerFactory = Guard.ThrowIfNull(commandHandlerFactory, nameof(commandHandlerFactory));
            _executorFactory = Guard.ThrowIfNull(executorFactory, nameof(executorFactory));
            _mainThreadContext = Guard.ThrowIfNull(mainThreadContext, nameof(mainThreadContext));
            _logger = Guard.ThrowIfNull(logger, nameof(logger));
        }

        public IMatchCommandPort Create(
            MatchState state,
            IMatchEventBus eventBus
        )
        {
            ValidateCreateArguments(state, eventBus);

            var visitor = _commandHandlerFactory.Create(state);
            var executor = _executorFactory.Create(
                state,
                eventBus,
                visitor
            );

            return new MatchCommandPort(
                executor,
                _mainThreadContext,
                _logger
            );
        }

        private static void ValidateCreateArguments(
            MatchState state,
            IMatchEventBus eventBus
        )
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(eventBus, nameof(eventBus));
        }
    }
}
