using System;
using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// αβ探索専用の合法手生成器。
    /// MatchState / IMatchCommand を生成せず、呼び出し元の再利用バッファへ SearchMove を直接書き込む。
    /// </summary>
    public sealed class SearchMoveGenerator
    {
        private const int BuiltInWallLength = 3;
        private readonly SearchPathfinder _pathfinder;
        private readonly Dictionary<PatternCacheKey, IReadOnlyList<WallPlacementPattern>> _patternCache = new();

        public SearchMoveGenerator(SearchPathfinder pathfinder)
        {
            _pathfinder = Guard.ThrowIfNull(pathfinder, nameof(pathfinder));
        }

        public void Generate(SearchState state, List<SearchMove> buffer)
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(buffer, nameof(buffer));

            buffer.Clear();
            var playerId = state.CurrentPlayerId;

            if (CanUseMovePawn(state, playerId))
            {
                AddMovePawnMoves(state, playerId, buffer);
            }

            if (CanUsePlaceWall(state, playerId))
            {
                AddPlaceWallMoves(state, playerId, buffer);
            }
        }

        private static bool CanUseMovePawn(SearchState state, PlayerId playerId)
        {
            return state.CanUseBuiltInSkill(playerId, BuiltInSkillSlotIds.MovePawn)
                && state.GetPlayer(playerId).Runtime.CanMove;
        }

        private static bool CanUsePlaceWall(SearchState state, PlayerId playerId)
        {
            return state.CanUseBuiltInSkill(playerId, BuiltInSkillSlotIds.PlaceWall)
                && state.GetPlayer(playerId).Runtime.CanPlaceWall;
        }

        private static void AddMovePawnMoves(SearchState state, PlayerId playerId, List<SearchMove> buffer)
        {
            var current = state.GetPawn(playerId);
            var opponent = state.GetPawn(playerId.Opponent);

            foreach (var direction in BoardDirections.FourDirections)
            {
                var next = BoardGeometry.Add(current, direction);

                if (!CanMoveOneTileIgnoringPawn(state, current, next))
                {
                    continue;
                }

                if (!next.Equals(opponent))
                {
                    AddMovePawnIfNotExists(buffer, playerId, next);
                    continue;
                }

                AddJumpOrSideMoves(state, playerId, opponent, direction, buffer);
            }
        }

        private static void AddJumpOrSideMoves(
            SearchState state,
            PlayerId playerId,
            Position opponent,
            (int dx, int dy) direction,
            List<SearchMove> buffer
        )
        {
            var jump = BoardGeometry.Add(opponent, direction);

            if (CanMoveOneTileIgnoringPawn(state, opponent, jump))
            {
                AddMovePawnIfNotExists(buffer, playerId, jump);
                return;
            }

            foreach (var sideDirection in BoardDirections.GetSideDirections(direction))
            {
                var side = BoardGeometry.Add(opponent, sideDirection);

                if (CanMoveOneTileIgnoringPawn(state, opponent, side))
                {
                    AddMovePawnIfNotExists(buffer, playerId, side);
                }
            }
        }

        private static void AddMovePawnIfNotExists(List<SearchMove> buffer, PlayerId playerId, Position target)
        {
            foreach (var move in buffer)
            {
                if (move.Kind == SearchMoveKind.MovePawn && move.Target.Equals(target))
                {
                    return;
                }
            }

            buffer.Add(SearchMove.MovePawn(playerId, target));
        }

        private void AddPlaceWallMoves(SearchState state, PlayerId playerId, List<SearchMove> buffer)
        {
            foreach (var pattern in GetPatterns(state.Width, state.Height, BuiltInWallLength))
            {
                if (!CanOverlayCandidateWall(state, pattern))
                {
                    continue;
                }

                var grid = new CandidateWallGrid(state, pattern.Cells);
                if (!_pathfinder.CanReachGoal(grid, state.Pawns, PlayerId.FirstPlayer))
                {
                    continue;
                }

                if (!_pathfinder.CanReachGoal(grid, state.Pawns, PlayerId.SecondPlayer))
                {
                    continue;
                }

                buffer.Add(SearchMove.PlaceWall(playerId, pattern.Origin, pattern.Direction, pattern.Cells));
            }
        }

        private IReadOnlyList<WallPlacementPattern> GetPatterns(int width, int height, int wallLength)
        {
            var key = new PatternCacheKey(width, height, wallLength);
            if (_patternCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var patterns = CreatePatterns(width, height, wallLength);
            _patternCache[key] = patterns;
            return patterns;
        }

        private static IReadOnlyList<WallPlacementPattern> CreatePatterns(int width, int height, int wallLength)
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

                    if (TryCreatePattern(origin, direction, wallLength, width, height, out var pattern))
                    {
                        patterns.Add(pattern);
                    }
                }
            }

            return patterns;
        }

        private static bool TryResolveDirection(Position origin, out WallDirection direction)
        {
            var xIsOdd = origin.X % 2 == 1;
            var yIsOdd = origin.Y % 2 == 1;

            if (xIsOdd == yIsOdd)
            {
                direction = default;
                return false;
            }

            direction = yIsOdd ? WallDirection.Horizontal : WallDirection.Vertical;
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

            pattern = new WallPlacementPattern(origin, direction, length, cells);
            return true;
        }

        private static bool CanOverlayCandidateWall(SearchState state, WallPlacementPattern pattern)
        {
            foreach (var cell in pattern.Cells)
            {
                if (!BoardGeometry.IsInside(cell, state.Width, state.Height))
                {
                    return false;
                }

                if (state.Get(cell.X, cell.Y) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanMoveOneTileIgnoringPawn(SearchState state, Position from, Position to)
        {
            if (!BoardGeometry.IsInside(to, state.Width, state.Height))
            {
                return false;
            }

            if (!BoardGeometry.IsTilePosition(to))
            {
                return false;
            }

            var middle = BoardGeometry.GetMiddle(from, to);
            if (!BoardGeometry.IsInside(middle, state.Width, state.Height))
            {
                return false;
            }

            return state.Get(middle.X, middle.Y) != 1;
        }

        private sealed class CandidateWallGrid : IReadOnlyIntGrid
        {
            private readonly SearchState _state;
            private readonly IReadOnlyList<Position> _candidateWalls;

            public CandidateWallGrid(SearchState state, IReadOnlyList<Position> candidateWalls)
            {
                _state = state;
                _candidateWalls = candidateWalls;
            }

            public int Width => _state.Width;
            public int Height => _state.Height;

            public int Get(int x, int y)
            {
                foreach (var wall in _candidateWalls)
                {
                    if (wall.X == x && wall.Y == y)
                    {
                        return 1;
                    }
                }

                return _state.Get(x, y);
            }
        }

        private readonly struct PatternCacheKey : IEquatable<PatternCacheKey>
        {
            private readonly int _width;
            private readonly int _height;
            private readonly int _wallLength;

            public PatternCacheKey(int width, int height, int wallLength)
            {
                _width = width;
                _height = height;
                _wallLength = wallLength;
            }

            public bool Equals(PatternCacheKey other)
            {
                return _width == other._width
                    && _height == other._height
                    && _wallLength == other._wallLength;
            }

            public override bool Equals(object obj)
            {
                return obj is PatternCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_width, _height, _wallLength);
            }
        }
    }
}
