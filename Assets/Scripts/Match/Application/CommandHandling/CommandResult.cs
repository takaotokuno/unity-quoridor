using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class CommandResult
    {
        public IReadOnlyList<IMatchEvent> Events { get; }
        public bool ConsumeTurn { get; }
        public bool RecordHistory { get; }

        public CommandResult(
            IReadOnlyList<IMatchEvent> events,
            bool consumeTurn,
            bool recordHistory = true)
        {
            Events = events ?? Array.Empty<IMatchEvent>();
            ConsumeTurn = consumeTurn;
            RecordHistory = recordHistory;
        }
    }
}
