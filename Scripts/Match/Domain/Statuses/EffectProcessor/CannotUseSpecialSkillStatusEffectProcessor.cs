using System.Collections.Generic;

namespace Quoridor
{
    public sealed class CannotUseSpecialSkillStatusEffectProcessor
        : IStatusEffectProcessor
    {
        public StatusEffectId EffectId => StatusEffectId.CannotUseSpecialSkill;

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            context.Player.Runtime.ProhibitSpecialSkill();

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