using System.Collections.Generic;

namespace Quoridor
{
    public static class CommandResultFactory
    {
        public static CommandResult FromStateChange(
            StateChangeResult stateChangeResult,
            bool consumeTurn,
            bool recordHistory = true
        )
        {
            Guard.ThrowIfNull(stateChangeResult, nameof(stateChangeResult));

            return new CommandResult(
                stateChangeResult.Events,
                consumeTurn,
                recordHistory
            );
        }

        public static CommandResult Reject(
            IMatchCommand command,
            string reason
        )
        {
            Guard.ThrowIfNull(command, nameof(command));

            Guard.ThrowIfNull(reason, nameof(reason));

            return new CommandResult(
                new IMatchEvent[]
                {
                    new CommandRejectedEvent(command, reason)
                },
                consumeTurn: false,
                recordHistory: false
            );
        }

        public static CommandResult FromEvents(
            IReadOnlyList<IMatchEvent> events,
            bool consumeTurn,
            bool recordHistory = true
        )
        {
            Guard.ThrowIfNull(events, nameof(events));

            return new CommandResult(
                events,
                consumeTurn,
                recordHistory
            );
        }
    }
}
