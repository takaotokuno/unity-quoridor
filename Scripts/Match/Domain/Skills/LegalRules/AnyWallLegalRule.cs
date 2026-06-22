using System.Collections.Generic;

namespace Quoridor
{
    public sealed class AnyWallLegalRule : ISkillLegalRule
    {
        public SkillLegalRuleId RuleId => SkillLegalRuleId.AnyWall;

        public bool IsLegal(SkillLegalContext context)
        {
            if (!context.Target.HasValue)
            {
                return false;
            }

            var board = context.State.Board;
            var target = context.Target.Value;

            return BoardGeometry.IsInside(board, target)
                && BoardGeometry.IsWallLinePosition(target);
        }

        public IReadOnlyList<Position> EnumerateLegalPositions(
            SkillLegalContext context
        )
        {
            var board = context.State.Board;
            var positions = new List<Position>();

            foreach (var position in BoardGeometry.EnumeratePositions(board))
            {
                if (BoardGeometry.IsWallLinePosition(position))
                {
                    positions.Add(position);
                }
            }

            return positions;
        }
    }
}