using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [System.Serializable]
    public sealed class SkillDefinitionEntry
    {
        [SerializeField] private string skillId;
        [SerializeField] private SkillActivationType activationType;
        [SerializeField] private SkillTargetKind targetKind;
        [SerializeField] private bool consumeTurn;
        [SerializeField] private int maxUseCount;
        [SerializeField] private string composerId;
        [SerializeField] private string ruleId;
        [SerializeField] private List<SkillParameterEntry> parameters = new();

        public SkillId SkillId => SkillId.Of(skillId);
        public SkillActivationType ActivationType => activationType;
        public SkillTargetKind TargetKind => targetKind;
        public bool ConsumeTurn => consumeTurn;
        public int MaxUseCount => maxUseCount;
        public SkillEffectComposerId ComposerId => SkillEffectComposerId.Of(composerId);
        public SkillLegalRuleId RuleId => SkillLegalRuleId.Of(ruleId);
        public IReadOnlyList<SkillParameterEntry> Parameters => parameters;
    }
}