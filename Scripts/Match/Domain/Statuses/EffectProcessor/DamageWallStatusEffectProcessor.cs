using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class DamageWallStatusEffectProcessor
        : IStatusEffectProcessor
    {
        public StatusEffectId EffectId => StatusEffectId.DamageWall;

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            var skill = context.Player.GetSkill(BuiltInSkillSlotIds.PlaceWall);

            if (!skill.RemainingUses.HasValue || skill.RemainingUses.Value <= 0)
            {
                return Array.Empty<IMatchEvent>();
            }

            var amount = context.EffectDefinition.GetInt(StatusParameterKeys.Amount, 1);
            skill.Consume(amount);

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