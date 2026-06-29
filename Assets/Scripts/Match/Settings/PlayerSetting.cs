using System.Collections.Generic;

namespace Quoridor
{
    public sealed class PlayerSetting
    {
        public bool IsCpu;
        public CpuAgentOptions CpuOptions = CpuAgentOptions.Default;
        public List<SkillId> SkillIds;
    }
}
