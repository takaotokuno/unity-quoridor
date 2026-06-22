using System.Collections.Generic;

namespace Quoridor
{
    public sealed class StatusDefinition
    {
        public StatusId StatusId { get; }
        public StatusReapplyPolicy ReapplyPolicy { get; }
        public IReadOnlyList<StatusEffectDefinition> Effects { get; }

        public StatusDefinition(
            StatusId statusId,
            StatusReapplyPolicy reapplyPolicy,
            IReadOnlyList<StatusEffectDefinition> effects
        )
        {
            StatusId = statusId;
            ReapplyPolicy = reapplyPolicy;
            Effects = effects;
        }
    }
}
