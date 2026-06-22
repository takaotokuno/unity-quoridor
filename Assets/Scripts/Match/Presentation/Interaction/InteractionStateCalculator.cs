using System.Collections.Generic;

namespace Quoridor
{
    public sealed class InteractionStateCalculator
    {
        private readonly ISkillDefinitionRegistry _skillDefinitionRegistry;
        private readonly ISkillLegalRuleRegistry _ruleRegistry;
        private readonly SkillAvailabilityValidator _skillAvailabilityValidator;

        public InteractionStateCalculator(
            ISkillDefinitionRegistry skillDefinitionRegistry,
            ISkillLegalRuleRegistry ruleRegistry,
            SkillAvailabilityValidator skillAvailabilityValidator
        )
        {
            _skillDefinitionRegistry = skillDefinitionRegistry;
            _ruleRegistry = ruleRegistry;
            _skillAvailabilityValidator = skillAvailabilityValidator;
        }

        public void CalculateForSkillButton(
            Dictionary<PlayerId, Dictionary<SkillSlotId, InteractionState>> skillButtonStates,
            MatchState state
        )
        {
            foreach (var playerPair in skillButtonStates)
            {
                PlayerId playerId = playerPair.Key;
                Dictionary<SkillSlotId, InteractionState> statesBySlot = playerPair.Value;

                foreach (var slotPair in statesBySlot)
                {
                    SkillSlotId skillSlotId = slotPair.Key;
                    InteractionState interactionState = slotPair.Value;

                    interactionState.IsActive = (state.Phase != MatchPhase.NotStarted);
                    interactionState.IsValid = false;

                    int? remainUses = state.GetPlayer(playerId).GetSkill(skillSlotId).RemainingUses; 
                    interactionState.RemainingUses = remainUses ?? 0;

                    if (!CanUseSkill(state, playerId, skillSlotId)) continue;

                    interactionState.IsValid = true;
                }
            }
        }

        public bool CanUseSkill(
            MatchState state,
            PlayerId playerId,
            SkillSlotId skillSlotId
        )
        {
            return _skillAvailabilityValidator.CanUse(
                state,
                playerId,
                skillSlotId
            );
        }

        public void CalculateForBoard(
            InteractionState[,] boardStates,
            MatchState state,
            PlayerId playerId,
            IReadOnlyList<SkillId> skillIds
        )
        {
            foreach (var value in boardStates)
            {
                value.IsValid = false;
            }

            foreach (var skillId in skillIds)
            {
                foreach (Position pos in EnumerateLegalPositions(
                    state,
                    playerId,
                    skillId
                ))
                {
                    boardStates[pos.Y, pos.X].IsValid = true;
                }
            }
        }

        private IReadOnlyList<Position> EnumerateLegalPositions(
            MatchState state,
            PlayerId playerId,
            SkillId skillId
        )
        {
            SkillDefinition definition = _skillDefinitionRegistry.Find(skillId);
            ISkillLegalRule rule = _ruleRegistry.Find(definition.RuleId);

            var context = new SkillLegalContext(
                state,
                playerId,
                definition,
                null
            );

            return rule.EnumerateLegalPositions(context);
        }
    }
}
