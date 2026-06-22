using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class WallPlacementPatternProvider
    {
        private readonly Dictionary<CacheKey, IReadOnlyList<WallPlacementPattern>> _cache = new();

        public IReadOnlyList<WallPlacementPattern> GetPatterns(BoardState board, int wallLength)
        {
            Guard.ThrowIfNull(board, nameof(board));

            var height = board.Grid.Height;
            var width = board.Grid.Width;

            var key = new CacheKey(width, height, wallLength);

            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var created = CreatePatterns(width, height, wallLength);
            _cache[key] = created;

            return created;
        }

        public bool TryGetPattern(
            BoardState board,
            int wallLength,
            Position origin,
            out WallPlacementPattern pattern
        )
        {
            Guard.ThrowIfNull(board, nameof(board));

            if (!TryResolveDirection(origin, out var direction))
            {
                pattern = default;
                return false;
            }

            return TryCreatePattern(
                origin,
                direction,
                wallLength,
                board.Grid.Width,
                board.Grid.Height,
                out pattern
            );
        }

        private static IReadOnlyList<WallPlacementPattern> CreatePatterns(
            int width,
            int height,
            int wallLength
        )
        {
            var patterns = new List<WallPlacementPattern>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var origin = new Position(x, y);

                    if (!TryResolveDirection(origin, out var direction))
                    {
                        continue;
                    }

                    if (TryCreatePattern(
                        origin,
                        direction,
                        wallLength,
                        width,
                        height,
                        out var pattern
                    ))
                    {
                        patterns.Add(pattern);
                    }
                }
            }

            return patterns;
        }

        private static bool TryResolveDirection(
            Position origin,
            out WallDirection direction
        )
        {
            var xIsOdd = origin.X % 2 == 1;
            var yIsOdd = origin.Y % 2 == 1;

            if (xIsOdd == yIsOdd)
            {
                direction = default;
                return false;
            }

            if (yIsOdd)
            {
                direction = WallDirection.Horizontal;
                return true;
            }

            direction = WallDirection.Vertical;
            return true;
        }

        private static bool TryCreatePattern(
            Position origin,
            WallDirection direction,
            int length,
            int width,
            int height,
            out WallPlacementPattern pattern
        )
        {
            if (length <= 0)
            {
                pattern = default;
                return false;
            }

            var cells = new Position[length];

            for (int i = 0; i < length; i++)
            {
                var x = origin.X + (direction == WallDirection.Horizontal ? i : 0);
                var y = origin.Y + (direction == WallDirection.Vertical ? i : 0);

                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    pattern = default;
                    return false;
                }

                cells[i] = new Position(x, y);
            }

            pattern = new WallPlacementPattern(
                origin,
                direction,
                length,
                cells
            );

            return true;
        }

        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            private readonly int _width;
            private readonly int _height;
            private readonly int _wallLength;

            public CacheKey(int width, int height, int wallLength)
            {
                _width = width;
                _height = height;
                _wallLength = wallLength;
            }

            public bool Equals(CacheKey other)
            {
                return _width == other._width
                    && _height == other._height
                    && _wallLength == other._wallLength;
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_width, _height, _wallLength);
            }
        }
    }
}