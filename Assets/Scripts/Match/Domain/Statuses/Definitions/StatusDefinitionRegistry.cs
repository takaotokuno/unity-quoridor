using System.Collections.Generic;

namespace Quoridor
{
    public sealed class StatusDefinitionRegistry : IStatusDefinitionRegistry
    {
        private readonly Dictionary<StatusId, StatusDefinition> _definitions = new();

        public StatusDefinitionRegistry(IEnumerable<StatusDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                _definitions.Add(definition.StatusId, definition);
            }
        }

        public StatusDefinition Find(StatusId statusId)
        {
            if (_definitions.TryGetValue(statusId, out var definition))
            {
                return definition;
            }

            throw new System.InvalidOperationException(
                $"SkillDefinition not found. SkillId: {statusId}"
            );
        }
    }
}