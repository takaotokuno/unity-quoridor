using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class TurnAdvancer
    {
        private const int MaxTurnSearchCount = 10;

        private readonly StatusEffectApplicator _statusApplicator;

        public TurnAdvancer(StatusEffectApplicator statusApplicator)
        {
            _statusApplicator = Guard.ThrowIfNull(statusApplicator, nameof(statusApplicator));
        }

        public IReadOnlyList<IMatchEvent> StartCurrentOrNextActableTurn(MatchState state)
        {
            ThrowIfStateIsNull(state);

            var events = new List<IMatchEvent>();

            ApplyCurrentTurnEffects(state, events);

            if (CurrentPlayerCanAct(state))
            {
                AppendTurnStartedEvent(state, events);
                return events;
            }

            AppendTurnSkippedEvent(state, events);

            return AdvanceToNextActableTurn(state, events);
        }

        public IReadOnlyList<IMatchEvent> AdvanceToNextActableTurn(MatchState state)
        {
            ThrowIfStateIsNull(state);

            return AdvanceToNextActableTurn(
                state,
                new List<IMatchEvent>()
            );
        }

        private IReadOnlyList<IMatchEvent> AdvanceToNextActableTurn(
            MatchState state,
            List<IMatchEvent> events
        )
        {
            for (int i = 0; i < MaxTurnSearchCount; i++)
            {
                AdvanceToNextTurn(state, events);

                if (CurrentPlayerCanAct(state))
                {
                    AppendTurnStartedEvent(state, events);
                    return events;
                }

                AppendTurnSkippedEvent(state, events);
            }

            throw new InvalidOperationException(
                "Can't find an actable player."
            );
        }

        private void AdvanceToNextTurn(
            MatchState state,
            List<IMatchEvent> events
        )
        {
            state.Turn.NextTurn();

            var currentPlayer = state.CurrentPlayer;
            events.AddRange(currentPlayer.OnTurnStarted().Events);
            events.AddRange(_statusApplicator.Apply(currentPlayer));
        }

        private void ApplyCurrentTurnEffects(
            MatchState state,
            List<IMatchEvent> events
        )
        {
            events.AddRange(_statusApplicator.Apply(state.CurrentPlayer));
        }

        private static bool CurrentPlayerCanAct(MatchState state)
        {
            return state.CurrentPlayer.Runtime.CanAct;
        }

        private static void AppendTurnStartedEvent(
            MatchState state,
            List<IMatchEvent> events
        )
        {
            var currentPlayer = state.CurrentPlayer;

            events.Add(new TurnStartedEvent(
                currentPlayer.PlayerId,
                state.CurrentTurn
            ));
        }

        private static void AppendTurnSkippedEvent(
            MatchState state,
            List<IMatchEvent> events
        )
        {
            var currentPlayer = state.CurrentPlayer;

            events.Add(new TurnSkippedEvent(
                currentPlayer.PlayerId,
                state.CurrentTurn
            ));
        }

        private static void ThrowIfStateIsNull(MatchState state)
        {
            Guard.ThrowIfNull(state, nameof(state));
        }
    }
}
