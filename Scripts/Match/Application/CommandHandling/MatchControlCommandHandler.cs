using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchControlCommandHandler
    {
        private readonly MatchState _state;
        private readonly TurnAdvancer _turnAdvancer;

        public MatchControlCommandHandler(
            MatchState state,
            TurnAdvancer turnAdvancer
        )
        {
            _state = Guard.ThrowIfNull(state, nameof(state));
            _turnAdvancer = Guard.ThrowIfNull(turnAdvancer, nameof(turnAdvancer));
        }

        public CommandResult Handle(MatchStartCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            var events = new List<IMatchEvent>();

            StateChangeResult stateChangeResult = _state.Start();
            events.AddRange(stateChangeResult.Events);
            events.AddRange(_turnAdvancer.StartCurrentOrNextActableTurn(_state));

            return CommandResultFactory.FromEvents(
                events,
                consumeTurn: false
            );
        }

        public CommandResult Handle(ResignCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            if (!_state.IsInProgress)
            {
                return CommandResultFactory.Reject(
                    command,
                    "Resign Rejected: Match is not in progress"
                );
            }

            return Finish(command.PlayerId.Opponent);
        }

        public CommandResult Handle(SkipCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            if (!_state.IsInProgress)
            {
                return CommandResultFactory.Reject(
                    command,
                    "Skip Rejected: Match is not in progress"
                );
            }

            return Finish(command.PlayerId);
        }

        public CommandResult Handle(TurnSkipCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            if (!_state.IsInProgress)
            {
                return CommandResultFactory.Reject(
                    command,
                    "Turn Skip Rejected: Match is not in progress"
                );
            }

            if (_state.CurrentPlayerId != command.PlayerId)
            {
                return CommandResultFactory.Reject(
                    command,
                    "Turn Skip Rejected: Player is not current"
                );
            }

            return CommandResultFactory.FromEvents(
                Array.Empty<IMatchEvent>(),
                consumeTurn: true
            );
        }

        private CommandResult Finish(PlayerId winner)
        {
            var events = new List<IMatchEvent>
            {
                new MatchWinnerDecidedEvent(winner)
            };

            StateChangeResult stateChangeResult = _state.Finish();

            events.AddRange(stateChangeResult.Events);

            return new CommandResult(
                events,
                consumeTurn: false
            );
        }
    }
}
