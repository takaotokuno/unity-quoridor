namespace Quoridor
{
    public interface ISkillEffectResolver
    {
        StateChangeResult Resolve(UseSkillCommand command, SkillDefinition definition);
    }
}
