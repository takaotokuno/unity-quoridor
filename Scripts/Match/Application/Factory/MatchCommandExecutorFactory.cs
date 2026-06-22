namespace Quoridor
{
    public sealed class MatchCommandExecutorFactory
    {
        private readonly MatchResultResolver _resultResolver;
        private readonly TurnAdvancer _turnAdvancer;
        private readonly DistanceCalculator _distanceCalculator;

        public MatchCommandExecutorFactory(
            MatchResultResolver resultResolver,
            TurnAdvancer turnAdvancer,
            DistanceCalculator distanceCalculator
        )
        {
            _resultResolver = Guard.ThrowIfNull(resultResolver, nameof(resultResolver));
            _turnAdvancer = Guard.ThrowIfNull(turnAdvancer, nameof(turnAdvancer));
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        }

        public MatchCommandExecutor Create(
            MatchState state,
            IMatchEventBus eventBus,
            CommandVisitor visitor
        )
        {
            ValidateCreateArguments(state, eventBus, visitor);

            var history = new MatchHistory(state.Capture());

            return new MatchCommandExecutor(
                state,
                history,
                eventBus,
                visitor,
                _resultResolver,
                _turnAdvancer,
                _distanceCalculator
            );
        }

        private static void ValidateCreateArguments(
            MatchState state,
            IMatchEventBus eventBus,
            CommandVisitor visitor
        )
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(eventBus, nameof(eventBus));
            Guard.ThrowIfNull(visitor, nameof(visitor));
        }
    }
}
