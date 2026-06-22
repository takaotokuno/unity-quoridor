using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchCommandExecutor
    {
        private readonly MatchState _state;
        private readonly MatchHistory _history;
        private readonly IMatchEventBus _eventBus;
        private readonly CommandVisitor _visitor;
        private readonly MatchResultResolver _resultResolver;
        private readonly TurnAdvancer _turnAdvancer;
        private readonly DistanceCalculator _distanceCalculator;

        public MatchCommandExecutor(
            MatchState state,
            MatchHistory history,
            IMatchEventBus eventBus,
            CommandVisitor visitor,
            MatchResultResolver resultResolver,
            TurnAdvancer turnAdvancer,
            DistanceCalculator distanceCalculator
        )
        {
            _state = Guard.ThrowIfNull(state, nameof(state));
            _history = Guard.ThrowIfNull(history, nameof(history));
            _eventBus = Guard.ThrowIfNull(eventBus, nameof(eventBus));
            _visitor = Guard.ThrowIfNull(visitor, nameof(visitor));
            _resultResolver = Guard.ThrowIfNull(resultResolver, nameof(resultResolver));
            _turnAdvancer = Guard.ThrowIfNull(turnAdvancer, nameof(turnAdvancer));
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        }

        public void Execute(IMatchCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            if (command is UndoCommand undoCommand)
            {
                ExecuteUndo(undoCommand);
                return;
            }

            if (command is RedoCommand redoCommand)
            {
                ExecuteRedo(redoCommand);
                return;
            }

            var events = new List<IMatchEvent>();

            CommandResult result = ValidateAutoPlayerInput(command) ?? ExecuteCore(command);
            events.AddRange(result.Events);

            if (result.ConsumeTurn)
            {
                events.AddRange(ResolveAfterTurnConsumed());
            }

            if (result.RecordHistory)
            {
                _history.Push(_state.Capture());
            }

            DispatchEvents(events);
        }

        private CommandResult ExecuteCore(IMatchCommand command)
        {
            return command.Execute(_visitor);
        }

        private CommandResult ValidateAutoPlayerInput(IMatchCommand command)
        {
            if (command is not PlayerCommandBase playerCommand)
                return null;

            if (playerCommand.Issuer != MatchCommandIssuers.InputPort)
                return null;

            PlayerState player = _state.GetPlayer(playerCommand.PlayerId);

            if (!player.Runtime.IsAuto)
                return null;

            return CommandResultFactory.Reject(
                command,
                "Command Rejected: Current player is auto"
            );
        }

        private IReadOnlyList<IMatchEvent> ResolveAfterTurnConsumed()
        {
            var events = new List<IMatchEvent>
            {
                new TurnEndedEvent(_state.CurrentPlayerId)
            };

            var distances = _distanceCalculator.Calculate(_state);
            events.Add(new DistanceUpdatedEvent(distances));

            if (_state.IsInProgress)
            {
                events.AddRange(_resultResolver.Resolve(_state, distances));
            }

            if (_state.IsInProgress)
            {
                events.AddRange(_turnAdvancer.AdvanceToNextActableTurn(_state));
            }

            return events;
        }

        private void ExecuteUndo(UndoCommand command)
        {
            if (!_history.TryUndo(out var memento))
                return;

            _state.Restore(memento);

            DispatchEvents(new IMatchEvent[]
            {
                new StateRestoredEvent(_state.Capture())
            });
        }

        private void ExecuteRedo(RedoCommand command)
        {
            if (!_history.TryRedo(out var memento))
                return;

            _state.Restore(memento);

            DispatchEvents(new IMatchEvent[]
            {
                new StateRestoredEvent(_state.Capture())
            });
        }

        private void DispatchEvents(IEnumerable<IMatchEvent> events)
        {
            foreach (var matchEvent in events)
            {
                if (matchEvent == null)
                    continue;

                matchEvent.Dispatch(_eventBus);
            }
        }
    }
}
