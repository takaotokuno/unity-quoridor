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
    }
}
