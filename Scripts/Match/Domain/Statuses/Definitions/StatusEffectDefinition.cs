using System.Collections.Generic;

namespace Quoridor
{
    public sealed class StatusEffectDefinition
    {
        public StatusEffectId EffectId { get; }
        public IReadOnlyDictionary<string, int> Parameters { get; }

        public StatusEffectDefinition(
            StatusEffectId effectId,
            IReadOnlyDictionary<string, int> parameters
        )
        {
            EffectId = effectId;
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