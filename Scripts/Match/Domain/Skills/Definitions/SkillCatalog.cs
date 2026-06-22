using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Quoridor/Skill Catalog")]
    public sealed class SkillCatalog : ScriptableObject
    {
        [SerializeField] private List<SkillDefinitionEntry> entries = new();

        public IReadOnlyList<SkillDefinitionEntry> Entries => entries;
    }
}