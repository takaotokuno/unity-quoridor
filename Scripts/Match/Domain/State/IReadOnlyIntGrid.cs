namespace Quoridor
{
    public interface IReadOnlyIntGrid
    {
        int Width { get; }
        int Height { get; }
        int Get(int x, int y);
    }
}
