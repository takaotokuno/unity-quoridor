using System.Collections.Generic;

namespace Quoridor
{
    public sealed class SkillDefinitionRegistry : ISkillDefinitionRegistry
    {
        private readonly Dictionary<SkillId, SkillDefinition> _definitions = new();

        public SkillDefinitionRegistry(IEnumerable<SkillDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                _definitions.Add(definition.SkillId, definition);
            }
        }

        public SkillDefinition Find(SkillId skillId)
        {
            if (_definitions.TryGetValue(skillId, out var definition))
            {
                return definition;
            }

            throw new System.InvalidOperationException(
                $"SkillDefinition not found. SkillId: {skillId}"
            );
        }
    }
}