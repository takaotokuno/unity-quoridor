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
            if (!CanOverlayCandidateWall(board, pattern))
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

        private static bool CanOverlayCandidateWall(
            BoardState board,
            WallPlacementPattern pattern
        )
        {
            if (board == null || board.Grid == null || pattern.Cells == null)
            {
                return false;
            }

            foreach (var cell in pattern.Cells)
            {
                if (!BoardGeometry.IsInside(cell, board.Grid.Width, board.Grid.Height))
                {
                    return false;
                }

                if (board.Grid.Get(cell.X, cell.Y) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private sealed class WallCandidateGrid : IReadOnlyIntGrid
        {
            private readonly IReadOnlyIntGrid _baseGrid;
            private readonly IReadOnlyList<Position> _candidateWalls;

            public WallCandidateGrid(
                IReadOnlyIntGrid baseGrid,
                IReadOnlyList<Position> candidateWalls
            )
            {
                _baseGrid = baseGrid;
                _candidateWalls = candidateWalls;
            }

            public int Width => _baseGrid.Width;

            public int Height => _baseGrid.Height;

            public int Get(int x, int y)
            {
                foreach (var wall in _candidateWalls)
                {
                    if (wall.X == x && wall.Y == y)
                    {
                        return 1;
                    }
                }

                return _baseGrid.Get(x, y);
            }
        }
    }
}
