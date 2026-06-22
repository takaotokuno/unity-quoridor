using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MovePawnValidator
    {
        public bool CanMovePawn(
            MatchState state,
            PlayerId playerId,
            Position to,
            int moveDistance
        )
        {
            var legalPositions = EnumerateLegalPositions(
                state,
                playerId,
                moveDistance
            );

            foreach (var position in legalPositions)
            {
                if (position.Equals(to)) return true;
            }

            return false;
        }

        public IReadOnlyList<Position> EnumerateLegalPositions(
            MatchState state,
            PlayerId playerId,
            int moveDistance
        )
        {
            var board = state.Board;

            if (moveDistance <= 0)
            {
                return new List<Position>();
            }

            if (moveDistance == 1)
            {
                return EnumerateNormalMovePositions(board, playerId);
            }

            return EnumerateFixedDistanceMovePositions(
                board,
                playerId,
                moveDistance
            );
        }

        public IEnumerable<(int, int)> EnumerateTiles(MatchState state)
        {
            var board = state.Board;
            var height = board.Grid.Height; 
            var width = board.Grid.Width;

            for (int y = 0; y < height; y += 2)
            {
                for (int x = 0; x < width; x += 2)
                {
                    yield return (x, y);
                }
            }
        }

        private static IReadOnlyList<Position> EnumerateNormalMovePositions(
            BoardState board,
            PlayerId playerId
        )
        {
            var result = new List<Position>();

            var current = PawnHelper.GetPawnPosition(board, playerId);
            var opponent = PawnHelper.GetOpponentPawnPosition(board, playerId);

            foreach (var direction in BoardDirections.FourDirections)
            {
                var next = BoardGeometry.Add(current, direction);

                if (!BoardGeometry.CanMoveOneTileIgnoringPawn(board, current, next))
                {
                    continue;
                }

                if (!next.Equals(opponent))
                {
                    AddIfNotExists(result, next);
                    continue;
                }

                AddJumpOrSideMoves(
                    result,
                    board,
                    opponent,
                    direction
                );
            }

            return result;
        }

        private static void AddJumpOrSideMoves(
            List<Position> result,
            BoardState board,
            Position opponent,
            (int dx, int dy) direction
        )
        {
            var jump = BoardGeometry.Add(opponent, direction);

            if (BoardGeometry.CanMoveOneTileIgnoringPawn(board, opponent, jump))
            {
                AddIfNotExists(result, jump);
                return;
            }

            foreach (var sideDirection in BoardDirections.GetSideDirections(direction))
            {
                var side = BoardGeometry.Add(opponent, sideDirection);

                if (BoardGeometry.CanMoveOneTileIgnoringPawn(board, opponent, side))
                {
                    AddIfNotExists(result, side);
                }
            }
        }

        private static IReadOnlyList<Position> EnumerateFixedDistanceMovePositions(
            BoardState board,
            PlayerId playerId,
            int moveDistance
        )
        {
            var result = new List<Position>();

            var current = PawnHelper.GetPawnPosition(board, playerId);
            var opponent = PawnHelper.GetOpponentPawnPosition(board, playerId);

            foreach (var direction in BoardDirections.FourDirections)
            {
                if (TryMoveFixedDistance(
                    board,
                    current,
                    opponent,
                    direction,
                    moveDistance,
                    out var destination
                ))
                {
                    AddIfNotExists(result, destination);
                }
            }

            return result;
        }

        private static bool TryMoveFixedDistance(
            BoardState board,
            Position from,
            Position opponent,
            (int dx, int dy) direction,
            int moveDistance,
            out Position destination
        )
        {
            var current = from;

            for (int step = 0; step < moveDistance; step++)
            {
                var next = BoardGeometry.Add(current, direction);

                if (!BoardGeometry.CanMoveOneTileIgnoringPawn(board, current, next))
                {
                    destination = default;
                    return false;
                }

                if (next.Equals(opponent))
                {
                    destination = default;
                    return false;
                }

                current = next;
            }

            destination = current;
            return true;
        }

        private static void AddIfNotExists(
            List<Position> positions,
            Position position
        )
        {
            foreach (var current in positions)
            {
                if (current.Equals(position))
                {
                    return;
                }
            }

            positions.Add(position);
        }
    }
}