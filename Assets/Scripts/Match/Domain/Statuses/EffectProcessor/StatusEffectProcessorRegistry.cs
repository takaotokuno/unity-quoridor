using System.Collections.Generic;

namespace Quoridor
{
    public sealed class StatusEffectProcessorRegistry : IStatusEffectProcessorRegistry
    {
        private readonly Dictionary<StatusEffectId, IStatusEffectProcessor> _processors;

        public StatusEffectProcessorRegistry(IEnumerable<IStatusEffectProcessor> processors)
        {
            _processors = new();
            foreach (var processor in processors)
            {
                _processors.Add(processor.EffectId, processor);
            }
        }

        public IStatusEffectProcessor Find(StatusEffectId effectId)
        {
            if (_processors.TryGetValue(effectId, out var processor))
            {
                return processor;
            }

            throw new System.InvalidOperationException(
                $"StatusEffectProcessor not found. EffectId: {effectId}"
            );
        }
    }
}