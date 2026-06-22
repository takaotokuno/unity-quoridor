namespace Quoridor
{
    public sealed class MatchStateConfig
    {
        public int BoardSize;
        public Position[] InitPawns;
        public PlayerConfig PlayerFirst;
        public PlayerConfig PlayerSecond;
        public PlayerSide StartingSide;
    }
}