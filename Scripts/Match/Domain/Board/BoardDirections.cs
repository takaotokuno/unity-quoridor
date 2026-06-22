using System.Collections.Generic;

namespace Quoridor
{
    public static class BoardDirections
    {
        public static readonly (int dx, int dy)[] FourDirections =
        {
            (0, -2),
            (0,  2),
            (-2, 0),
            (2,  0),
        };

        public static readonly (int dx, int dy)[] HorizontalSideDirections =
        {
            (-2, 0),
            (2, 0),
        };

        public static readonly (int dx, int dy)[] VerticalSideDirections =
        {
            (0, -2),
            (0, 2),
        };

        public static IReadOnlyList<(int dx, int dy)> GetSideDirections(
            (int dx, int dy) direction
        )
        {
            return direction.dx == 0
                ? HorizontalSideDirections
                : VerticalSideDirections;
        }
    }
}