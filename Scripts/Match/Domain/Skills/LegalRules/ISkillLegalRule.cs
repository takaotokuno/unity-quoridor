using System.Collections.Generic;

namespace Quoridor
{
    public interface ISkillLegalRule
    {
        SkillLegalRuleId RuleId { get; }

        bool IsLegal(SkillLegalContext context);

        IReadOnlyList<Position> EnumerateLegalPositions(SkillLegalContext context);
    }
}