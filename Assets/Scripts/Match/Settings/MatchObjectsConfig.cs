using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchObjectsConfig
    {
        public int BoardSize;
        public Position[] InitPawns;
        public IReadOnlyList<SkillId> SkillIdsFirst;
        public IReadOnlyList<SkillId> SkillIdsSecond;
        public MatchViewPrefabCatalog ViewPrefabCatalog;
        public ObjectLayoutView ObjectLayoutView;
    }
}