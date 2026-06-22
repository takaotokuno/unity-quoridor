using System;

namespace Quoridor
{
    public sealed class PlaceWallEffectComposer : ISkillEffectComposer
    {
        private readonly WallPlacementPatternProvider _patternProvider;

        public SkillEffectComposerId ComposerId => SkillEffectComposerId.Of("place_wall");

        public PlaceWallEffectComposer(
            WallPlacementPatternProvider patternProvider
        )
        {
            _patternProvider = patternProvider;
        }

        public StateChangeResult Compose(SkillEffectContext context)
        {
            if (!context.Target.HasValue)
            {
                throw new InvalidOperationException(
                    "PlaceWall requires target position."
                );
            }

            var boardState = context.State.Board;
            var wallLength = context.Definition.GetInt(SkillParameterKeys.Length);

            if (!_patternProvider.TryGetPattern(
                boardState,
                wallLength,
                context.Target.Value,
                out var pattern
            ))
            {
                throw new InvalidOperationException(
                    "PlaceWall target position does not match a valid wall pattern."
                );
            }

            return boardState.PlaceWall(pattern.Cells);
        }
    }
}