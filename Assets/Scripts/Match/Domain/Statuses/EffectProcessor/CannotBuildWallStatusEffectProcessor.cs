using System.Collections.Generic;

namespace Quoridor
{
    public sealed class CannotPlaceWallStatusEffectProcessor
        : IStatusEffectProcessor
    {
        public StatusEffectId EffectId => StatusEffectId.CannotPlaceWall;

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            context.Player.Runtime.ProhibitWallPlacement();

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