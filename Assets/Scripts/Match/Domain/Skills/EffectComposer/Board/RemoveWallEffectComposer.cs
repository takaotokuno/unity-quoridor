using System;

namespace Quoridor
{
    public sealed class RemoveWallEffectComposer : ISkillEffectComposer
    {
        private readonly ExistingWallPatternResolver _existingWallPatternResolver;

        public SkillEffectComposerId ComposerId => SkillEffectComposerId.Of("remove_wall");

        public RemoveWallEffectComposer(
            ExistingWallPatternResolver existingWallPatternResolver
        )
        {
            _existingWallPatternResolver = existingWallPatternResolver;
        }

        public StateChangeResult Compose(SkillEffectContext context)
        {
            if (!context.Target.HasValue)
            {
                throw new InvalidOperationException(
                    "RemoveWall requires target position."
                );
            }

            var board = context.State.Board;
            var target = context.Target.Value;
            var wallLength = context.Definition.GetInt(SkillParameterKeys.Length);

            if (!BoardGeometry.IsInside(board, target))
            {
                throw new InvalidOperationException(
                    "RemoveWall target position is outside the board."
                );
            }

            if (!BoardGeometry.IsWallLinePosition(target))
            {
                throw new InvalidOperationException(
                    "RemoveWall target position is not a wall line."
                );
            }

            if (!BoardGeometry.IsWall(board, target))
            {
                throw new InvalidOperationException(
                    "RemoveWall target position is not a wall."
                );
            }

            if (!_existingWallPatternResolver.TryResolve(
                board,
                wallLength,
                target,
                out var pattern
            ))
            {
                throw new InvalidOperationException(
                    "RemoveWall target position does not match a valid wall pattern."
                );
            }

            return board.RemoveWall(pattern.Cells);
        }
    }
}
