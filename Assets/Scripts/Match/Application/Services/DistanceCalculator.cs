using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class DistanceCalculator
    {
        private const int MaxCachedSnapshots = 4096;
        private readonly Pathfinder _pathfinder;
        private readonly GoalResolver _goalResolver;
        private readonly Dictionary<BoardDistanceCacheKey, DistanceSnapshot> _distanceCache = new();
        private readonly Queue<BoardDistanceCacheKey> _cacheInsertionOrder = new();

        public DistanceCalculator(
            Pathfinder pathfinder,
            GoalResolver goalResolver
        )
        {
            _pathfinder = pathfinder;
            _goalResolver = goalResolver;
        }

        public DistanceSnapshot Calculate(MatchState state)
        {
            var cacheKey = BoardDistanceCacheKey.Create(state.Board);
            if (_distanceCache.TryGetValue(cacheKey, out DistanceSnapshot cachedDistances))
            {
                return cachedDistances;
            }

            var firstDistance = CalculateDistance(
                state.Board,
                PlayerId.FirstPlayer
            );

            var secondDistance = CalculateDistance(
                state.Board,
                PlayerId.SecondPlayer
            );

            var distances = new DistanceSnapshot(
                firstDistance,
                secondDistance
            );
            StoreCachedDistance(cacheKey, distances);
            return distances;
        }

        private void StoreCachedDistance(BoardDistanceCacheKey cacheKey, DistanceSnapshot distances)
        {
            if (_distanceCache.ContainsKey(cacheKey))
            {
                _distanceCache[cacheKey] = distances;
                return;
            }

            _distanceCache.Add(cacheKey, distances);
            _cacheInsertionOrder.Enqueue(cacheKey);

            while (_distanceCache.Count > MaxCachedSnapshots && _cacheInsertionOrder.Count > 0)
            {
                _distanceCache.Remove(_cacheInsertionOrder.Dequeue());
            }
        }

        private int CalculateDistance(
            BoardState board,
            PlayerId playerId
        )
        {
            var start = PawnHelper.GetPawnPosition(board, playerId);

            if (_goalResolver.IsGoal(board, playerId, start))
            {
                return 0;
            }

            return _pathfinder.GetShortestDistanceToGoal(
                board,
                playerId
            );
        }

        private readonly struct BoardDistanceCacheKey : IEquatable<BoardDistanceCacheKey>
        {
            private readonly int _width;
            private readonly int _height;
            private readonly int _firstPawnX;
            private readonly int _firstPawnY;
            private readonly int _secondPawnX;
            private readonly int _secondPawnY;
            private readonly ulong _gridHash;

            private BoardDistanceCacheKey(BoardState board)
            {
                _width = board.Grid.Width;
                _height = board.Grid.Height;
                Position firstPawn = board.Pawns[PlayerId.FirstPlayer.ToIndex()];
                Position secondPawn = board.Pawns[PlayerId.SecondPlayer.ToIndex()];
                _firstPawnX = firstPawn.X;
                _firstPawnY = firstPawn.Y;
                _secondPawnX = secondPawn.X;
                _secondPawnY = secondPawn.Y;
                _gridHash = CalculateGridHash(board.Grid);
            }

            public static BoardDistanceCacheKey Create(BoardState board)
            {
                Guard.ThrowIfNull(board, nameof(board));
                return new BoardDistanceCacheKey(board);
            }

            public bool Equals(BoardDistanceCacheKey other)
            {
                return _width == other._width
                    && _height == other._height
                    && _firstPawnX == other._firstPawnX
                    && _firstPawnY == other._firstPawnY
                    && _secondPawnX == other._secondPawnX
                    && _secondPawnY == other._secondPawnY
                    && _gridHash == other._gridHash;
            }

            public override bool Equals(object obj)
            {
                return obj is BoardDistanceCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(
                    _width,
                    _height,
                    _firstPawnX,
                    _firstPawnY,
                    _secondPawnX,
                    _secondPawnY,
                    _gridHash
                );
            }

            private static ulong CalculateGridHash(IReadOnlyIntGrid grid)
            {
                const ulong offsetBasis = 14695981039346656037UL;
                const ulong prime = 1099511628211UL;
                ulong hash = offsetBasis;

                for (var y = 0; y < grid.Height; y++)
                {
                    for (var x = 0; x < grid.Width; x++)
                    {
                        hash ^= (uint)(grid.Get(x, y) + 1);
                        hash *= prime;
                    }
                }

                return hash;
            }
        }

    }
}
