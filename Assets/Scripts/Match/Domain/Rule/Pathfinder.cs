using System.Collections.Generic;

namespace Quoridor
{
    public class Pathfinder
    {
        private readonly GoalResolver _resolver;
        public Pathfinder(
            GoalResolver resolver
        )
        {
            _resolver = resolver;
        }

        public bool CanReachGoal(BoardState board, PlayerId playerId)
        {
            if (board == null)
            {
                return false;
            }

            return CanReachGoal(board.Grid, board.Pawns, playerId);
        }

        public bool CanReachGoal(
            IReadOnlyIntGrid grid,
            IReadOnlyList<Position> pawns,
            PlayerId playerId
        )
        {
            if (!TryGetReachabilityStart(grid, pawns, playerId, out var start))
            {
                return false;
            }

            return CanReachGoalFromPosition(grid, start, playerId);
        }

        private bool CanReachGoalFromPosition(
            IReadOnlyIntGrid grid,
            Position start,
            PlayerId playerId
        )
        {
            var height = grid.Height;
            var width = grid.Width;

            var visited = new bool[height, width];
            var queue = new Queue<Position>();

            visited[start.Y, start.X] = true;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (_resolver.IsGoal(height, playerId, current))
                {
                    return true;
                }

                foreach (var direction in BoardDirections.FourDirections)
                {
                    var next = BoardGeometry.Add(current, direction);

                    if (!BoardGeometry.IsInside(next, width, height))
                    {
                        continue;
                    }

                    if (visited[next.Y, next.X])
                    {
                        continue;
                    }

                    if (!CanMoveOneTileIgnoringPawn(grid, current, next))
                    {
                        continue;
                    }

                    visited[next.Y, next.X] = true;
                    queue.Enqueue(next);
                }
            }

            return false;
        }

        public int GetShortestDistanceToGoal(BoardState board, PlayerId playerId)
        {
            if (board == null)
            {
                return -1;
            }

            return GetShortestDistanceToGoal(board.Grid, board.Pawns, playerId);
        }

        public int GetShortestDistanceToGoal(
            IReadOnlyIntGrid grid,
            IReadOnlyList<Position> pawns,
            PlayerId playerId
        )
        {
            if (!TryGetReachabilityStart(grid, pawns, playerId, out var start))
            {
                return -1;
            }

            return GetShortestDistanceFromPosition(grid, start, playerId);
        }

        private int GetShortestDistanceFromPosition(
            IReadOnlyIntGrid grid,
            Position start,
            PlayerId playerId
        )
        {
            var height = grid.Height;
            var width = grid.Width;

            var distance = new int[height, width];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    distance[y, x] = -1;
                }
            }

            var queue = new Queue<Position>();

            distance[start.Y, start.X] = 0;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (_resolver.IsGoal(height, playerId, current))
                {
                    return distance[current.Y, current.X];
                }

                foreach (var direction in BoardDirections.FourDirections)
                {
                    var next = BoardGeometry.Add(current, direction);

                    if (!BoardGeometry.IsInside(next, width, height))
                    {
                        continue;
                    }

                    if (distance[next.Y, next.X] != -1)
                    {
                        continue;
                    }

                    if (!CanMoveOneTileIgnoringPawn(grid, current, next))
                    {
                        continue;
                    }

                    distance[next.Y, next.X] = distance[current.Y, current.X] + 1;
                    queue.Enqueue(next);
                }
            }

            return -1;
        }

        private static bool TryGetReachabilityStart(
            IReadOnlyIntGrid grid,
            IReadOnlyList<Position> pawns,
            PlayerId playerId,
            out Position start
        )
        {
            start = default(Position);

            if (grid == null)
            {
                return false;
            }

            if (playerId == null || playerId.Value <= 0)
            {
                return false;
            }

            if (pawns == null || pawns.Count < playerId.Value)
            {
                return false;
            }

            start = pawns[playerId.ToIndex()];

            if (!BoardGeometry.IsInside(start, grid.Width, grid.Height))
            {
                return false;
            }

            if (!BoardGeometry.IsTilePosition(start))
            {
                return false;
            }

            return true;
        }

        private static bool CanMoveOneTileIgnoringPawn(
            IReadOnlyIntGrid grid,
            Position from,
            Position to
        )
        {
            if (!BoardGeometry.IsInside(to, grid.Width, grid.Height))
            {
                return false;
            }

            if (!BoardGeometry.IsTilePosition(to))
            {
                return false;
            }

            var middle = BoardGeometry.GetMiddle(from, to);

            if (!BoardGeometry.IsInside(middle, grid.Width, grid.Height))
            {
                return false;
            }

            return grid.Get(middle.X, middle.Y) != 1;
        }
    }
}
