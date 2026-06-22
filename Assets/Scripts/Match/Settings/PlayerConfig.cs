using System.Collections.Generic;

namespace Quoridor
{
    public sealed class PlayerConfig
    {
        public bool IsCpu;
        public CpuAgentStrategyKind CpuStrategyKind;
        public IReadOnlyList<SkillId> SkillIds;

        public PlayerConfig(
            bool isCpu,
            IReadOnlyList<SkillId> skillIds,
            CpuAgentStrategyKind cpuStrategyKind = CpuAgentStrategyKind.RandomLegal
        )
        {
            IsCpu = isCpu;
            CpuStrategyKind = cpuStrategyKind;
            SkillIds = skillIds;
        }
    }
}