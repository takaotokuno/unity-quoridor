using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchResultResolver
    {
        private readonly GoalResolver _goalResolver;
        private readonly CheckmateResolver _checkmateResolver;

        public MatchResultResolver(
            GoalResolver goalResolver,
            CheckmateResolver checkmateResolver
        )
        {
            _goalResolver = goalResolver;
            _checkmateResolver = checkmateResolver;
        }

        public IReadOnlyList<IMatchEvent> Resolve(
            MatchState state,
            DistanceSnapshot distances
        )
        {
            var events = new List<IMatchEvent>();

            var goalWinnerId = _goalResolver.FindWinner(state.Board);
            if (goalWinnerId != null)
            {
                return FinishMatch(state, goalWinnerId, events);
            }

            var checkmateWinnerId = _checkmateResolver.FindWinner(
                state,
                distances
            );

            if (checkmateWinnerId != null)
            {
                events.Add(new CheckmateEvent(checkmateWinnerId.Opponent));
                return FinishMatch(state, checkmateWinnerId, events);
            }

            return events;
        }

        private IReadOnlyList<IMatchEvent> FinishMatch(
            MatchState state,
            PlayerId winnerId,
            List<IMatchEvent> events
        )
        {
            var result = state.Finish();

            events.Add(new MatchWinnerDecidedEvent(winnerId));
            events.AddRange(result.Events);

            return events;
        }
    }
}
