using System.Collections.Generic;

namespace Quoridor
{
    public sealed class PlayerConfig
    {
        public bool IsCpu;
        public CpuAgentOptions CpuOptions;
        public IReadOnlyList<SkillId> SkillIds;

        public PlayerConfig(
            bool isCpu,
            IReadOnlyList<SkillId> skillIds,
            CpuAgentOptions cpuOptions = null
        )
        {
            IsCpu = isCpu;
            CpuOptions = cpuOptions ?? CpuAgentOptions.Default;
            SkillIds = skillIds;
        }
    }
}