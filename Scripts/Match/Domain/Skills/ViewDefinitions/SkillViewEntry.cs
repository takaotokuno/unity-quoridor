using UnityEngine;

namespace Quoridor
{
    [System.Serializable]
    public sealed class SkillViewEntry
    {
        [SerializeField] private string skillId;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        public SkillId SkillId => SkillId.Of(skillId);
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
    }
}
