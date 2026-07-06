using System.Collections.Generic;

namespace Quoridor
{
    public class Pathfinder
    {
        private readonly SearchPathfinder _searchPathfinder;

        public Pathfinder(
            GoalResolver resolver,
            SearchProfiler searchProfiler
        )
        {
            _searchPathfinder = new SearchPathfinder(
                Guard.ThrowIfNull(resolver, nameof(resolver)),
                Guard.ThrowIfNull(searchProfiler, nameof(searchProfiler))
            );
        }

        public bool CanReachGoal(BoardState board, PlayerId playerId)
        {
            return _searchPathfinder.CanReachGoal(board, playerId);
        }

        public bool CanReachGoal(
            IReadOnlyIntGrid grid,
            IReadOnlyList<Position> pawns,
            PlayerId playerId
        )
        {
            return _searchPathfinder.CanReachGoal(grid, pawns, playerId);
        }

        public int GetShortestDistanceToGoal(BoardState board, PlayerId playerId)
        {
            return _searchPathfinder.GetShortestDistanceToGoal(board, playerId);
        }

        public int GetShortestDistanceToGoal(
            IReadOnlyIntGrid grid,
            IReadOnlyList<Position> pawns,
            PlayerId playerId
        )
        {
            return _searchPathfinder.GetShortestDistanceToGoal(grid, pawns, playerId);
        }
    }
}
