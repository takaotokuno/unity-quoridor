using System.Collections.Generic;

namespace Quoridor
{
    public interface IStatusEffectProcessor
    {
        StatusEffectId EffectId { get; }

        IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context);
    }
}