using System.Collections.Generic;

namespace Quoridor
{
    internal sealed class WallCandidateGrid : IReadOnlyIntGrid
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
