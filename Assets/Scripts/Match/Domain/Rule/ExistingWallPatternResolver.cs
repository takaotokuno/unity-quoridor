namespace Quoridor
{
    public sealed class ExistingWallPatternResolver
    {
        private readonly WallPlacementPatternProvider _patternProvider;

        public ExistingWallPatternResolver(
            WallPlacementPatternProvider patternProvider
        )
        {
            _patternProvider = patternProvider;
        }

        public bool TryResolve(
            BoardState board,
            int wallLength,
            Position target,
            out WallPlacementPattern pattern
        )
        {
            Guard.ThrowIfNull(board, nameof(board));

            if (wallLength <= 0
                || !BoardGeometry.IsInside(board, target)
                || !BoardGeometry.IsWallLinePosition(target)
                || !BoardGeometry.IsWall(board, target))
            {
                pattern = default;
                return false;
            }

            if (!_patternProvider.TryGetPattern(
                board,
                wallLength,
                target,
                out var candidate
            ))
            {
                pattern = default;
                return false;
            }

            if (!IsExistingWall(board, candidate))
            {
                pattern = default;
                return false;
            }

            pattern = candidate;
            return true;
        }

        private static bool IsExistingWall(
            BoardState board,
            WallPlacementPattern pattern
        )
        {
            foreach (var cell in pattern.Cells)
            {
                if (!BoardGeometry.IsWall(board, cell))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
