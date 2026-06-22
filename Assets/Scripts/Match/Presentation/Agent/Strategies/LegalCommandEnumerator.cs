using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// 現在の MatchState から CPU が発行可能な合法コマンド候補を列挙する責務を持つ。
    /// </summary>
    public sealed class LegalCommandEnumerator
    {
        private readonly SkillAvailabilityValidator _skillAvailabilityValidator;
        private readonly ISkillDefinitionRegistry _skillDefinitionRegistry;
        private readonly ISkillLegalRuleRegistry _skillLegalRuleRegistry;

        public LegalCommandEnumerator(
            SkillAvailabilityValidator skillAvailabilityValidator,
            ISkillDefinitionRegistry skillDefinitionRegistry,
            ISkillLegalRuleRegistry skillLegalRuleRegistry
        )
        {
            _skillAvailabilityValidator = Guard.ThrowIfNull(
                skillAvailabilityValidator,
                nameof(skillAvailabilityValidator)
            );
            _skillDefinitionRegistry = Guard.ThrowIfNull(skillDefinitionRegistry, nameof(skillDefinitionRegistry));
            _skillLegalRuleRegistry = Guard.ThrowIfNull(skillLegalRuleRegistry, nameof(skillLegalRuleRegistry));
        }

        /// <summary>
        /// 指定されたカテゴリに該当する合法コマンドをすべて列挙する。
        /// </summary>
        public List<IMatchCommand> Enumerate(
            CpuAgentDecisionContext context,
            LegalCommandEnumerationOptions options
        )
        {
            var candidates = new List<IMatchCommand>();

            if (options.IncludeMovePawn)
            {
                AddMovePawnCommands(candidates, context);
            }

            if (options.IncludePlaceWall)
            {
                AddPlaceWallCommands(candidates, context);
            }

            if (options.IncludeSpecialSkills)
            {
                AddSpecialSkillCommands(candidates, context);
            }

            return candidates;
        }

        private void AddMovePawnCommands(List<IMatchCommand> candidates, CpuAgentDecisionContext context)
        {
            if (!_skillAvailabilityValidator.CanUse(context.State, context.PlayerId, BuiltInSkillSlotIds.MovePawn))
                return;

            AddUseSkillCommands(candidates, context, BuiltInSkillSlotIds.MovePawn, allowTargetless: false);
        }

        private void AddPlaceWallCommands(List<IMatchCommand> candidates, CpuAgentDecisionContext context)
        {
            if (!_skillAvailabilityValidator.CanUse(context.State, context.PlayerId, BuiltInSkillSlotIds.PlaceWall))
                return;

            AddUseSkillCommands(candidates, context, BuiltInSkillSlotIds.PlaceWall, allowTargetless: false);
        }

        private void AddSpecialSkillCommands(List<IMatchCommand> candidates, CpuAgentDecisionContext context)
        {
            var player = context.State.GetPlayer(context.PlayerId);
            foreach (var kvp in player.Skills)
            {
                var slotId = kvp.Key;
                if (slotId.Value <= BuiltInSkillSlotIds.PlaceWall.Value)
                    continue;

                if (!_skillAvailabilityValidator.CanUse(context.State, context.PlayerId, slotId))
                    continue;

                AddUseSkillCommands(candidates, context, slotId, allowTargetless: true);
            }
        }

        private void AddUseSkillCommands(
            List<IMatchCommand> candidates,
            CpuAgentDecisionContext context,
            SkillSlotId slotId,
            bool allowTargetless
        )
        {
            var skill = context.State.GetPlayer(context.PlayerId).Skills[slotId];
            var skillDef = _skillDefinitionRegistry.Find(skill.SkillId);
            var rule = _skillLegalRuleRegistry.Find(skillDef.RuleId);
            var legalContext = new SkillLegalContext(context.State, context.PlayerId, skillDef, null);
            var legalPositions = rule.EnumerateLegalPositions(legalContext);

            if (legalPositions.Count == 0)
            {
                if (allowTargetless)
                {
                    candidates.Add(new UseSkillCommand(context.PlayerId, slotId, null, context.Issuer));
                }

                return;
            }

            foreach (var pos in legalPositions)
            {
                candidates.Add(new UseSkillCommand(context.PlayerId, slotId, pos, context.Issuer));
            }
        }
    }
}
