using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// 探索中に大量実行される BFS 用の Pathfinder。
    /// visited / distance / queue の作業領域をインスタンス内で再利用し、
    /// BFS ごとの配列生成と全マス初期化を避ける。
    /// </summary>
    public sealed class SearchPathfinder
    {
        private readonly GoalResolver _resolver;
        private readonly SearchProfiler _searchProfiler;
        private int[] _distance;
        private int[] _visitedStamp;
        private Position[] _queue;
        private int _stamp;
        private int _width;
        private int _height;

        public SearchPathfinder(
            GoalResolver resolver,
            SearchProfiler searchProfiler
        )
        {
            _resolver = Guard.ThrowIfNull(resolver, nameof(resolver));
            _searchProfiler = Guard.ThrowIfNull(searchProfiler, nameof(searchProfiler));
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

            return GetShortestDistanceFromPosition(grid, start, playerId, false) >= 0;
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

            return GetShortestDistanceFromPosition(grid, start, playerId, true);
        }

        private int GetShortestDistanceFromPosition(
            IReadOnlyIntGrid grid,
            Position start,
            PlayerId playerId,
            bool returnDistance
        )
        {
            _searchProfiler.RecordBfsSearch();

            EnsureCapacity(grid.Width, grid.Height);
            var stamp = NextStamp();
            var head = 0;
            var tail = 0;
            var startIndex = ToIndex(start.X, start.Y);

            _visitedStamp[startIndex] = stamp;
            _distance[startIndex] = 0;
            _queue[tail++] = start;

            while (head < tail)
            {
                var current = _queue[head++];
                var currentIndex = ToIndex(current.X, current.Y);

                if (_resolver.IsGoal(_height, playerId, current))
                {
                    return returnDistance ? _distance[currentIndex] : 0;
                }

                foreach (var direction in BoardDirections.FourDirections)
                {
                    var next = BoardGeometry.Add(current, direction);

                    if (!BoardGeometry.IsInside(next, _width, _height))
                    {
                        continue;
                    }

                    if (!BoardGeometry.IsTilePosition(next))
                    {
                        continue;
                    }

                    var nextIndex = ToIndex(next.X, next.Y);
                    if (_visitedStamp[nextIndex] == stamp)
                    {
                        continue;
                    }

                    if (!CanMoveOneTileIgnoringPawn(grid, current, next))
                    {
                        continue;
                    }

                    _visitedStamp[nextIndex] = stamp;
                    _distance[nextIndex] = _distance[currentIndex] + 1;
                    _queue[tail++] = next;
                }
            }

            return -1;
        }

        private void EnsureCapacity(int width, int height)
        {
            var capacity = width * height;
            if (_distance != null && _distance.Length >= capacity)
            {
                _width = width;
                _height = height;
                return;
            }

            _distance = new int[capacity];
            _visitedStamp = new int[capacity];
            _queue = new Position[capacity];
            _width = width;
            _height = height;
        }

        private int NextStamp()
        {
            if (_stamp == int.MaxValue)
            {
                System.Array.Clear(_visitedStamp, 0, _visitedStamp.Length);
                _stamp = 0;
            }

            _stamp += 1;
            return _stamp;
        }

        private int ToIndex(int x, int y)
        {
            return y * _width + x;
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
            var middle = BoardGeometry.GetMiddle(from, to);

            if (!BoardGeometry.IsInside(middle, grid.Width, grid.Height))
            {
                return false;
            }

            return grid.Get(middle.X, middle.Y) != 1;
        }
    }
}
