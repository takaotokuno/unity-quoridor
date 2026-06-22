namespace Quoridor
{
    public interface ISkillEffectComposer
    {
        SkillEffectComposerId ComposerId { get; }

        StateChangeResult Compose(SkillEffectContext context);
    }
}
