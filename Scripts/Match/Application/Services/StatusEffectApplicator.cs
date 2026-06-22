using System.Collections.Generic;

namespace Quoridor{
    public sealed class StatusEffectApplicator
    {
        private readonly IStatusDefinitionRegistry _definitionRegistry;
        private readonly IStatusEffectProcessorRegistry _processorRegistry;

        public StatusEffectApplicator(
            IStatusDefinitionRegistry definitionRegistry,
            IStatusEffectProcessorRegistry processorRegistry
        )
        {
            _definitionRegistry = Guard.ThrowIfNull(definitionRegistry, nameof(definitionRegistry));
            _processorRegistry = Guard.ThrowIfNull(processorRegistry, nameof(processorRegistry));
        }

        public IReadOnlyList<IMatchEvent> Apply(PlayerState player)
        {
            List<IMatchEvent> events = new();
            HashSet<StatusId> appliedStatusIds = new();

            player.Runtime.Reset();

            foreach (var status in player.Statuses)
            {
                if (!status.IsReady)
                    continue;

                var definition = _definitionRegistry.Find(status.StatusId);

                if (definition.ReapplyPolicy != StatusReapplyPolicy.Stack)
                {
                    if (!appliedStatusIds.Add(status.StatusId))
                        continue;
                }

                foreach (var effect in definition.Effects)
                {
                    var processor = _processorRegistry.Find(effect.EffectId);

                    events.AddRange(processor.Apply(
                        new StatusEffectContext(
                            player.PlayerId,
                            player,
                            status.StatusId,
                            effect
                        )
                    ));
                }
            }

            return events;
        }
    }
}
