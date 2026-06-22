namespace Quoridor
{
    public static class SkillButtonSlotMapper
    {
        public const int FirstSkillButtonSlotId = 2;

        public static int GetSkillButtonCount(int skillCount)
        {
            if (skillCount <= FirstSkillButtonSlotId) return 0;
            return skillCount - FirstSkillButtonSlotId;
        }

        public static SkillSlotId ToSkillSlotId(int buttonIndex)
        {
            return new SkillSlotId(buttonIndex + 1 + FirstSkillButtonSlotId);
        }

        public static int ToButtonIndex(SkillSlotId skillSlotId)
        {
            return skillSlotId.Value - FirstSkillButtonSlotId - 1;
        }
    }
}
