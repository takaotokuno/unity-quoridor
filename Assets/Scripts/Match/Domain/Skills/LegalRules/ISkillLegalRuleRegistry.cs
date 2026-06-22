namespace Quoridor
{
    public interface ISkillLegalRuleRegistry
    {
        ISkillLegalRule Find(SkillLegalRuleId ruleId);
    }
}