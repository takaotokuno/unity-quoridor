using System.Collections.Generic;

namespace Quoridor
{
    public sealed class PlayerSetting
    {
        public bool IsCpu;
        public CpuAgentStrategyKind CpuStrategyKind = CpuAgentStrategyKind.RandomLegal;
        public List<SkillId> SkillIds;
    }   
}