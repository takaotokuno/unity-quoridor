using System.Collections.Generic;

namespace Quoridor
{
    public sealed class SkillDefinition
    {
        public SkillId SkillId { get; }
        public SkillActivationType Type { get; }
        public SkillTargetKind Kind { get; }
        public bool ConsumeTurn { get; }
        public int MaxUseCount { get; }
        public SkillEffectComposerId ComposerId { get; }
        public SkillLegalRuleId RuleId { get; }
        public IReadOnlyDictionary<string, int> Parameters { get; }

        public SkillDefinition(
            SkillId skillId,
            SkillActivationType type,
            SkillTargetKind kind,
            bool consumeTurn,
            int maxUseCount,
            SkillEffectComposerId composerId,
            SkillLegalRuleId ruleId,
            IReadOnlyDictionary<string, int> parameters
        )
        {
            SkillId = skillId;
            Type = type;
            Kind = kind;
            ConsumeTurn = consumeTurn;
            MaxUseCount = maxUseCount;
            ComposerId = composerId;
            RuleId = ruleId;
            Parameters = parameters;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (Parameters.TryGetValue(key, out int value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}