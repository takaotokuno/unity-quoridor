using System.Collections.Generic;

namespace Quoridor
{
    public sealed class PlaceWallValidator
    {
        private readonly Pathfinder _pathfinder;

        public PlaceWallValidator(Pathfinder pathfinder)
        {
            _pathfinder = pathfinder;
        }

        public bool CanPlaceWall(
            MatchState state,
            WallPlacementPattern pattern
        )
        {
            var board = state.Board;
            if (InterferesWithExistingWall(board, pattern))
            {
                return false;
            }

            var gridWithCandidateWall = new WallCandidateGrid(
                board.Grid,
                pattern.Cells
            );

            if (!_pathfinder.CanReachGoal(
                gridWithCandidateWall,
                board.Pawns,
                PlayerId.FirstPlayer
            ))
            {
                return false;
            }

            if (!_pathfinder.CanReachGoal(
                gridWithCandidateWall,
                board.Pawns,
                PlayerId.SecondPlayer
            ))
            {
                return false;
            }

            return true;
        }

        private static bool InterferesWithExistingWall(
            BoardState board,
            WallPlacementPattern pattern
        )
        {
            foreach (var cell in pattern.Cells)
            {
                if (board.Grid.Get(cell.X, cell.Y) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private sealed class WallCandidateGrid : IReadOnlyIntGrid
        {
            private readonly IReadOnlyIntGrid _baseGrid;
            private readonly HashSet<int> _candidateWallIndices;

            public WallCandidateGrid(
                IReadOnlyIntGrid baseGrid,
                IReadOnlyList<Position> candidateWalls
            )
            {
                _baseGrid = baseGrid;
                _candidateWallIndices = new HashSet<int>();

                foreach (var wall in candidateWalls)
                {
                    _candidateWallIndices.Add(ToIndex(wall.X, wall.Y));
                }
            }

            public int Width => _baseGrid.Width;

            public int Height => _baseGrid.Height;

            public int Get(int x, int y)
            {
                if (_candidateWallIndices.Contains(ToIndex(x, y)))
                {
                    return 1;
                }

                return _baseGrid.Get(x, y);
            }

            private int ToIndex(int x, int y)
            {
                return y * Width + x;
            }
        }
    }
}
