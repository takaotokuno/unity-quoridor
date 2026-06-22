using System.Collections.Generic;

namespace Quoridor
{
    public static class SkillCatalogConverter
    {
        public static IReadOnlyList<SkillDefinition> Convert(SkillCatalog catalog)
        {
            List<SkillDefinition> definitions = new();

            foreach (var entry in catalog.Entries)
            {
                Dictionary<string, int> parameters = new();

                foreach (var parameter in entry.Parameters)
                {
                    parameters[parameter.Key] = parameter.Value;
                }

                definitions.Add(new SkillDefinition(
                    entry.SkillId,
                    entry.ActivationType,
                    entry.TargetKind,
                    entry.ConsumeTurn,
                    entry.MaxUseCount,
                    entry.ComposerId,
                    entry.RuleId,
                    parameters
                ));
            }

            return definitions;
        }
    }
}