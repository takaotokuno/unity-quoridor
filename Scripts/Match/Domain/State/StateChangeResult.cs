using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor
{
    public sealed class StateChangeResult
    {
        public static readonly StateChangeResult Empty =
            new(Array.Empty<IMatchEvent>());

        public IReadOnlyList<IMatchEvent> Events { get; }

        public bool HasEvents => Events.Count > 0;

        private StateChangeResult(IReadOnlyList<IMatchEvent> events)
        {
            Events = Guard.ThrowIfNull(events, nameof(events));
        }

        public static StateChangeResult NoChange()
        {
            return Empty;
        }

        public static StateChangeResult Changed(IMatchEvent matchEvent)
        {
            Guard.ThrowIfNull(matchEvent, nameof(matchEvent));

            return new StateChangeResult(new[] { matchEvent });
        }

        public static StateChangeResult Changed(params IMatchEvent[] events)
        {
            Guard.ThrowIfNull(events, nameof(events));

            if (events.Length == 0)
                return Empty;

            ValidateNoNullEvents(events, nameof(events));

            return new StateChangeResult((IMatchEvent[])events.Clone());
        }

        public static StateChangeResult Changed(IEnumerable<IMatchEvent> events)
        {
            Guard.ThrowIfNull(events, nameof(events));

            IMatchEvent[] array = events.ToArray();

            if (array.Length == 0)
                return Empty;

            ValidateNoNullEvents(array, nameof(events));

            return new StateChangeResult(array);
        }

        private static void ValidateNoNullEvents(
            IReadOnlyList<IMatchEvent> events,
            string paramName
        )
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i] == null)
                {
                    throw new ArgumentException(
                        "Events must not contain null.",
                        paramName
                    );
                }
            }
        }

        public static StateChangeResult Merge(params StateChangeResult[] results)
        {
            Guard.ThrowIfNull(results, nameof(results));

            List<IMatchEvent> events = new();

            foreach (StateChangeResult result in results)
            {
                if (result == null)
                    throw new ArgumentException("Results must not contain null.", nameof(results));

                events.AddRange(result.Events);
            }

            return Changed(events);
        }

        public static StateChangeResult Merge(IEnumerable<StateChangeResult> results)
        {
            Guard.ThrowIfNull(results, nameof(results));

            List<IMatchEvent> events = new();

            foreach (StateChangeResult result in results)
            {
                if (result == null)
                    throw new ArgumentException("Results must not contain null.", nameof(results));

                events.AddRange(result.Events);
            }

            return Changed(events);
        }
    }
}
