using System.Collections.Generic;

namespace Quoridor
{
    public sealed class RemoveWallLegalRule : ISkillLegalRule
    {
        private readonly ExistingWallPatternResolver _existingWallPatternResolver;

        public SkillLegalRuleId RuleId { get; } = SkillLegalRuleId.Of("remove_wall");

        public RemoveWallLegalRule(
            ExistingWallPatternResolver existingWallPatternResolver
        )
        {
            _existingWallPatternResolver = existingWallPatternResolver;
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

            return _existingWallPatternResolver.TryResolve(
                board,
                wallLength,
                target,
                out _
            );
        }

        public IReadOnlyList<Position> EnumerateLegalPositions(
            SkillLegalContext context
        )
        {
            var board = context.State.Board;
            var wallLength = context.Definition.GetInt(SkillParameterKeys.Length);
            var positions = new List<Position>();

            foreach (var position in BoardGeometry.EnumeratePositions(board))
            {
                if (!_existingWallPatternResolver.TryResolve(
                    board,
                    wallLength,
                    position,
                    out _
                ))
                {
                    continue;
                }

                positions.Add(position);
            }

            return positions;
        }
    }
}