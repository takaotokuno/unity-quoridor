namespace Quoridor
{
    public sealed class MatchSetting
    {
        public int BoardSize;
        public Position[] InitPawns;
        public PlayerSide StartingSide;
        public PlayerSetting PlayerFirst;
        public PlayerSetting PlayerSecond;
        public MatchViewPrefabCatalog ViewPrefabCatalog; 
        public ObjectLayoutView ObjectLayoutView;
    }
}