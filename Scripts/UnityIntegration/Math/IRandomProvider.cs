namespace Quoridor
{
    public interface IRandomProvider
    {
        int Range(int minInclusive, int maxExclusive);
        bool RollPercent(int percent);
    }
}