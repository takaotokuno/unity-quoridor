using System;
using System.Collections.Generic;

namespace Quoridor
{
    public readonly struct WallPlacementPattern
    {
        public Position Origin { get; }
        public WallDirection Direction { get; }
        public int Length { get; }
        public IReadOnlyList<Position> Cells { get; }

        public WallPlacementPattern(
            Position origin,
            WallDirection direction,
            int length,
            Position[] cells
        )
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            Guard.ThrowIfNull(cells, nameof(cells));

            if (cells.Length != length)
            {
                throw new ArgumentException(
                    "Wall placement pattern cells count must match length.",
                    nameof(cells)
                );
            }

            for (int i = 0; i < cells.Length; i++)
            {
                var expectedX = origin.X + (direction == WallDirection.Horizontal ? i : 0);
                var expectedY = origin.Y + (direction == WallDirection.Vertical ? i : 0);

                if (!cells[i].Equals(new Position(expectedX, expectedY)))
                {
                    throw new ArgumentException(
                        "Wall placement pattern cells must be contiguous from origin in direction.",
                        nameof(cells)
                    );
                }
            }

            Origin = origin;
            Direction = direction;
            Length = length;
            Cells = (Position[])cells.Clone();
        }

        public bool Contains(Position target)
        {
            foreach (var cell in Cells)
            {
                if (cell.Equals(target))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
