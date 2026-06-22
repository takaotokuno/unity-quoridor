namespace Quoridor
{
    public enum SkillAvailabilityRejectReason
    {
        None,
        MatchNotInProgress,
        NotCurrentPlayer,
        SpecialSkillRestricted,
        SkillSlotNotFound,
        CoolingDown,
        NoRemainingUses,
        TileSkillRestricted,
        WallSkillRestricted
    }
}
