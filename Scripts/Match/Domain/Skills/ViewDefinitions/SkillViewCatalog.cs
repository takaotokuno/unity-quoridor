using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Quoridor/Skill View Catalog")]
    public sealed class SkillViewCatalog : ScriptableObject
    {
        [SerializeField] private List<SkillViewEntry> entries = new();

        public SkillViewEntry Find(SkillId skillId)
        {
            foreach (var entry in entries)
            {
                if (entry.SkillId == skillId)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}