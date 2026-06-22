namespace Quoridor
{
    public interface ISkillEffectComposerRegistry
    {
        ISkillEffectComposer Find(SkillEffectComposerId composerId);
    }
}
