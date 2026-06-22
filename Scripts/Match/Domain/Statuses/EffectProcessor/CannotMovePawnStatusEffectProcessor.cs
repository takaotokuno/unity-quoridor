using System.Collections.Generic;

namespace Quoridor
{
    public sealed class CannotMovePawnStatusEffectProcessor
        : IStatusEffectProcessor
    {
        public StatusEffectId EffectId => StatusEffectId.CannotMovePawn;

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            context.Player.Runtime.ProhibitMove();

            return new IMatchEvent[]
            {
                new StatusAppliedEvent(
                    context.PlayerId,
                    context.StatusId
                )
            };
        }
    }
}