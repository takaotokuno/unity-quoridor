using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MovePawnLegalRule : ISkillLegalRule
    {
        private readonly MovePawnValidator _validator;

        public SkillLegalRuleId RuleId { get; } = SkillLegalRuleId.Of("move_pawn");

        public MovePawnLegalRule(MovePawnValidator validator)
        {
            _validator = validator;
        }

        public bool IsLegal(SkillLegalContext context)
        {
            if (context.Target == null)
            {
                return false;
            }

            var moveDistance = context.Definition.GetInt(SkillParameterKeys.Distance);

            return _validator.CanMovePawn(
                context.State,
                context.PlayerId,
                context.Target.Value,
                moveDistance
            );
        }

        public IReadOnlyList<Position> EnumerateLegalPositions(
            SkillLegalContext context
        )
        {
            var moveDistance = context.Definition.GetInt(SkillParameterKeys.Distance);

            return _validator.EnumerateLegalPositions(
                context.State,
                context.PlayerId,
                moveDistance
            );
        }
    }
}