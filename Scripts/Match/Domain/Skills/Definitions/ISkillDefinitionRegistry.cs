namespace Quoridor
{
    public interface ISkillDefinitionRegistry
    {
        SkillDefinition Find(SkillId skillId);
    }
}

