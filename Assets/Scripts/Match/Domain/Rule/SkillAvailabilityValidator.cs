namespace Quoridor
{
    public sealed class SkillAvailabilityValidator
    {
        private readonly ISkillDefinitionRegistry _definitionRegistry;
        
        public SkillAvailabilityValidator(
            ISkillDefinitionRegistry definitionRegistry
        )
        {
            _definitionRegistry = definitionRegistry;   
        }

        public bool CanUse(MatchState state, PlayerId playerId, SkillSlotId skillSlotId)
        {
            return Evaluate(state, playerId, skillSlotId).CanUse;
        }

        public SkillAvailabilityResult Evaluate(
            MatchState state,
            PlayerId playerId,
            SkillSlotId skillSlotId
        )
        {
            Guard.ThrowIfNull(state, nameof(state));

            Guard.ThrowIfNull(playerId, nameof(playerId));

            Guard.ThrowIfNull(skillSlotId, nameof(skillSlotId));

            if (state.Phase != MatchPhase.InProgress)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.MatchNotInProgress,
                    "Match is not in progress"
                );
            }

            if (state.CurrentPlayerId != playerId)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.NotCurrentPlayer,
                    "Not current player"
                );
            }

            PlayerState player = state.GetPlayer(playerId);

            bool isSpecialSkill = skillSlotId.Value >= BuiltInSkillSlotIds.SpecialSkill.Value;
            if (isSpecialSkill && !player.Runtime.CanUseSpecialSkill)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.SpecialSkillRestricted,
                    "Special skill use is restricted"
                );
            }

            if (!player.TryGetSkillBySlotId(skillSlotId, out var skill))
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.SkillSlotNotFound,
                    "Skill slot was not found"
                );
            }

            if (skill.CoolDownRemaining > 0)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.CoolingDown,
                    "Skill is cooling down"
                );
            }

            if (skill.RemainingUses == 0)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.NoRemainingUses,
                    "Skill has no remaining uses"
                );
            }

            var definition = _definitionRegistry.Find(skill.SkillId);
            if (!player.Runtime.CanMove && definition.Kind == SkillTargetKind.Tile)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.TileSkillRestricted,
                    "Tile skill use is restricted"
                );
            }

            if (!player.Runtime.CanPlaceWall && definition.Kind == SkillTargetKind.Wall)
            {
                return SkillAvailabilityResult.Reject(
                    SkillAvailabilityRejectReason.WallSkillRestricted,
                    "Wall skill use is restricted"
                );
            }

            return SkillAvailabilityResult.Available();
        }
    }
}
