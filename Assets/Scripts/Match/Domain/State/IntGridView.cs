namespace Quoridor
{
    public sealed class IntGridView : IReadOnlyIntGrid
    {
        private readonly int[,] _values;

        public IntGridView(int[,] values)
        {
            _values = Guard.ThrowIfNull(values, nameof(values));
        }

        public int Width
        {
            get { return _values.GetLength(1); }
        }

        public int Height
        {
            get { return _values.GetLength(0); }
        }

        public int Get(int x, int y)
        {
            return _values[y, x];
        }

        public int this[int y, int x]
        {
            get { return _values[y, x]; }
        }
    }
}
