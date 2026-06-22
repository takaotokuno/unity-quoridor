using System.Collections.Generic;

namespace Quoridor
{
    public sealed class CannotActStatusEffectProcessor : IStatusEffectProcessor
    {
        public StatusEffectId EffectId => StatusEffectId.CannotAct;

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            context.Player.Runtime.ProhibitAction();
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