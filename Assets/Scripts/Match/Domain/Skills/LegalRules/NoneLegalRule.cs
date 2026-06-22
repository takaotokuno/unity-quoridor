using System.Collections.Generic;

namespace Quoridor
{
    public sealed class NoneLegalRule : ISkillLegalRule
    {
        public SkillLegalRuleId RuleId { get; } = SkillLegalRuleId.Of("none");

        public bool IsLegal(SkillLegalContext context)
        {
            return true;
        }

        public IReadOnlyList<Position> EnumerateLegalPositions(
            SkillLegalContext context
        )
        {
            return new Position[0];
        }
    }
}