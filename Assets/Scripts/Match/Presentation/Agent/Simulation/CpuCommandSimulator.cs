namespace Quoridor
{
    /// <summary>
    /// CPU 探索中に本物の MatchState を汚さずコマンド結果を評価するための仮実行器。
    /// </summary>
    public sealed class CpuCommandSimulator
    {
        private readonly CommandHandlerFactory _commandHandlerFactory;
        private readonly MatchResultResolver _resultResolver;
        private readonly TurnAdvancer _turnAdvancer;
        private readonly DistanceCalculator _distanceCalculator;

        public CpuCommandSimulator(
            CommandHandlerFactory commandHandlerFactory,
            MatchResultResolver resultResolver,
            TurnAdvancer turnAdvancer,
            DistanceCalculator distanceCalculator
        )
        {
            _commandHandlerFactory = Guard.ThrowIfNull(commandHandlerFactory, nameof(commandHandlerFactory));
            _resultResolver = Guard.ThrowIfNull(resultResolver, nameof(resultResolver));
            _turnAdvancer = Guard.ThrowIfNull(turnAdvancer, nameof(turnAdvancer));
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        }

        public CpuCommandSimulationResult Simulate(MatchState state, IMatchCommand command)
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(command, nameof(command));

            MatchState copiedState = CopyState(state);
            CommandResult result = ExecuteCopiedState(copiedState, command);

            return result.ConsumeTurn
                ? CpuCommandSimulationResult.Success(copiedState)
                : CpuCommandSimulationResult.Failure();
        }

        public CpuCommandSimulationResult SimulateTurn(MatchState state, IMatchCommand command)
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(command, nameof(command));

            MatchState copiedState = CopyState(state);
            CommandResult result = ExecuteCopiedState(copiedState, command);
            if (!result.ConsumeTurn)
                return CpuCommandSimulationResult.Failure();

            ResolveAfterTurnConsumed(copiedState);

            return CpuCommandSimulationResult.Success(copiedState);
        }

        private CommandResult ExecuteCopiedState(MatchState copiedState, IMatchCommand command)
        {
            CommandVisitor visitor = _commandHandlerFactory.Create(copiedState);
            return command.Execute(visitor);
        }

        private void ResolveAfterTurnConsumed(MatchState copiedState)
        {
            var distances = _distanceCalculator.Calculate(copiedState);

            if (copiedState.IsInProgress)
            {
                _resultResolver.Resolve(copiedState, distances);
            }

            if (copiedState.IsInProgress)
            {
                _turnAdvancer.AdvanceToNextActableTurn(copiedState);
            }
        }

        private static MatchState CopyState(MatchState state)
        {
            MatchMemento memento = state.Capture();

            return new MatchState(
                memento.Board,
                memento.Players,
                memento.Turn,
                memento.Phase
            );
        }
    }
}
