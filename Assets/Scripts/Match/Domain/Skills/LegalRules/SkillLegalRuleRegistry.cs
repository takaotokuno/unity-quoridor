using System.Collections.Generic;

namespace Quoridor
{
    public sealed class SkillLegalRuleRegistry : ISkillLegalRuleRegistry
    {
        private readonly Dictionary<SkillLegalRuleId, ISkillLegalRule> _rules = new();

        public SkillLegalRuleRegistry(IEnumerable<ISkillLegalRule> rules)
        {
            foreach (var rule in rules)
            {
                if (!_rules.TryAdd(rule.RuleId, rule))
                {
                    throw new System.InvalidOperationException(
                        $"Duplicate SkillLegalTargetRule. RuleId: {rule.RuleId}"
                    );
                }
            }
        }

        public ISkillLegalRule Find(SkillLegalRuleId ruleId)
        {
            if (_rules.TryGetValue(ruleId, out var rule))
            {
                return rule;
            }

            throw new System.InvalidOperationException(
                $"SkillLegalTargetRule not found. RuleId: {ruleId}"
            );
        }
    }
}