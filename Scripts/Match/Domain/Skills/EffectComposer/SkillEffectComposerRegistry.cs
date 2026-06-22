using System.Collections.Generic;

namespace Quoridor
{
    public sealed class SkillEffectComposerRegistry : ISkillEffectComposerRegistry
    {
        private readonly Dictionary<SkillEffectComposerId, ISkillEffectComposer> _composers = new();

        public SkillEffectComposerRegistry(IEnumerable<ISkillEffectComposer> composers)
        {
            foreach (var composer in composers)
            {
                _composers.Add(composer.ComposerId, composer);
            }
        }

        public ISkillEffectComposer Find(SkillEffectComposerId composerId)
        {
            if (_composers.TryGetValue(composerId, out var composer))
            {
                return composer;
            }

            throw new System.InvalidOperationException(
                $"SkillEffectComposer not found. ComposerId: {composerId}"
            );
        }
    }
}
