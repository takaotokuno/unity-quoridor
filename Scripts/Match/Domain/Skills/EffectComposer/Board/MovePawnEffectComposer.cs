using System;

namespace Quoridor
{
    public sealed class MovePawnEffectComposer : ISkillEffectComposer
    {
        public SkillEffectComposerId ComposerId => SkillEffectComposerId.Of("move_pawn");

        public StateChangeResult Compose(SkillEffectContext context)
        {
            Guard.ThrowIfNull(context, nameof(context));

            if (!context.Target.HasValue)
                throw new InvalidOperationException("MovePawn requires target position.");

            return context.State.Board.MovePawn(
                context.PlayerId,
                context.Target.Value
            );
        }
    }
}
