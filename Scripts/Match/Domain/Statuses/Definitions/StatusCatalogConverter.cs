using System.Collections.Generic;

namespace Quoridor
{
    public static class StatusCatalogConverter
    {
        public static IReadOnlyList<StatusDefinition> Convert(StatusCatalog catalog)
        {
            List<StatusDefinition> definitions = new();

            foreach (var entry in catalog.Entries)
            {
                List<StatusEffectDefinition> effects = new();

                foreach (var effectEntry in entry.EffectEntries)
                {
                    Dictionary<string, int> parameters = new();

                    foreach (var parameter in effectEntry.Parameters)
                    {
                        parameters[parameter.Key] = parameter.Value;
                    }

                    effects.Add(new StatusEffectDefinition(
                        effectEntry.EffectId,
                        parameters
                    ));
                }

                definitions.Add(new StatusDefinition(
                    entry.StatusId,
                    entry.ReapplyPolicy,
                    effects
                ));
            }

            return definitions;
        }
    }
}