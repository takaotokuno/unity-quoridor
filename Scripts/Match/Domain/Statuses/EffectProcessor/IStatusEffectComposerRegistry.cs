namespace Quoridor
{
    public interface IStatusEffectProcessorRegistry
    {
        IStatusEffectProcessor Find(StatusEffectId composerId);
    }
}

