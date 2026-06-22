using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class RecoveryWallStatusEffectProcessor
        : IStatusEffectProcessor
    {
        public StatusEffectId EffectId => StatusEffectId.RecoveryWall;

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            var skill = context.Player.Skills[BuiltInSkillSlotIds.PlaceWall];

            if (!skill.RemainingUses.HasValue)
            {
                return Array.Empty<IMatchEvent>();
            }

            var amount = context.EffectDefinition.GetInt(StatusParameterKeys.Amount, 1);
            skill.Recover(amount);

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