using System.Collections.Generic;

namespace Quoridor
{
    public static class BoardGeometry
    {
        public static bool IsInside(BoardState board, Position position)
        {
            return IsInside(
                position,
                board.Grid.Width,
                board.Grid.Height
            );
        }

        public static bool IsInside(
            Position position,
            int width,
            int height
        )
        {
            return position.X >= 0
                && position.X < width
                && position.Y >= 0
                && position.Y < height;
        }

        public static bool IsTilePosition(Position position)
        {
            return position.X % 2 == 0
                && position.Y % 2 == 0;
        }

        public static bool IsWallPosition(Position position)
        {
            return position.X % 2 == 1
                || position.Y % 2 == 1;
        }

        public static bool IsWallLinePosition(Position position)
        {
            var xIsOdd = position.X % 2 == 1;
            var yIsOdd = position.Y % 2 == 1;

            return xIsOdd ^ yIsOdd;
        }

        public static bool IsIntersectionPosition(Position position)
        {
            return position.X % 2 == 1
                && position.Y % 2 == 1;
        }

        public static bool IsWall(BoardState board, Position position)
        {
            return board.Grid.Get(position.X, position.Y) == 1;
        }

        public static bool IsEmpty(BoardState board, Position position)
        {
            return board.Grid.Get(position.X, position.Y) == 0;
        }

        public static Position GetMiddle(Position from, Position to)
        {
            return new Position(
                (from.X + to.X) / 2,
                (from.Y + to.Y) / 2
            );
        }

        public static Position Add(Position position, (int dx, int dy) direction)
        {
            return new Position(
                position.X + direction.dx,
                position.Y + direction.dy
            );
        }

        public static bool CanMoveOneTileIgnoringPawn(
            BoardState board,
            Position from,
            Position to
        )
        {
            if (!IsInside(board, to))
            {
                return false;
            }

            if (!IsTilePosition(to))
            {
                return false;
            }

            var middle = GetMiddle(from, to);

            if (!IsInside(board, middle))
            {
                return false;
            }

            return !IsWall(board, middle);
        }

        public static IEnumerable<Position> EnumeratePositions(BoardState board)
        {
            for (int y = 0; y < board.Grid.Height; y++)
            {
                for (int x = 0; x < board.Grid.Width; x++)
                {
                    yield return new Position(x, y);
                }
            }
        }
    }
}