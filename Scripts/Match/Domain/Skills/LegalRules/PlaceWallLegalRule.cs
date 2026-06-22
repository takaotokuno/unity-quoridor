using System.Collections.Generic;

namespace Quoridor
{
    public sealed class PlaceWallLegalRule : ISkillLegalRule
    {
        private readonly PlaceWallValidator _validator;
        private readonly WallPlacementPatternProvider _patternProvider;

        public SkillLegalRuleId RuleId { get; } = SkillLegalRuleId.Of("place_wall");

        public PlaceWallLegalRule(
            PlaceWallValidator validator,
            WallPlacementPatternProvider patternProvider
        )
        {
            _validator = validator;
            _patternProvider = patternProvider;
        }

        public bool IsLegal(SkillLegalContext context)
        {
            if (!context.Target.HasValue)
            {
                return false;
            }

            var board = context.State.Board;
            var target = context.Target.Value;
            var wallLength = context.Definition.GetInt(SkillParameterKeys.Length);

            if (!BoardGeometry.IsInside(board, target))
            {
                return false;
            }

            if (!BoardGeometry.IsWallLinePosition(target))
            {
                return false;
            }

            if (!_patternProvider.TryGetPattern(
                board,
                wallLength,
                target,
                out var pattern
            ))
            {
                return false;
            }

            return _validator.CanPlaceWall(
                context.State,
                pattern
            );
        }

        public IReadOnlyList<Position> EnumerateLegalPositions(
            SkillLegalContext context
        )
        {
            var board = context.State.Board;
            var wallLength = context.Definition.GetInt(SkillParameterKeys.Length);
            var patterns = _patternProvider.GetPatterns(board, wallLength);

            var legalPositions = new List<Position>();

            foreach (var pattern in patterns)
            {
                if (_validator.CanPlaceWall(
                    context.State,
                    pattern
                ))
                {
                    legalPositions.Add(pattern.Origin);
                }
            }

            return legalPositions;
        }
    }
}