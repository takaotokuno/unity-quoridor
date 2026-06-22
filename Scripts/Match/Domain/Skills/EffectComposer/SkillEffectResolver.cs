namespace Quoridor
{
    public sealed class SkillEffectResolver : ISkillEffectResolver
    {
        private readonly MatchState _state;
        private readonly ISkillEffectComposerRegistry _composerRegistry;

        public SkillEffectResolver(
            MatchState state,
            ISkillEffectComposerRegistry composerRegistry
        )
        {
            _state = Guard.ThrowIfNull(state, nameof(state));
            _composerRegistry = Guard.ThrowIfNull(composerRegistry, nameof(composerRegistry));
        }

        public StateChangeResult Resolve(
            UseSkillCommand command,
            SkillDefinition definition
        )
        {
            Guard.ThrowIfNull(command, nameof(command));

            Guard.ThrowIfNull(definition, nameof(definition));

            ISkillEffectComposer composer =
                _composerRegistry.Find(definition.ComposerId);

            SkillEffectContext context = new SkillEffectContext(
                _state,
                command.PlayerId,
                definition,
                command.Target
            );

            var effectResult = composer.Compose(context);
            var useSkillResult = _state
                .GetPlayer(command.PlayerId)
                .UseSkill(command.SkillSlotId);

            return StateChangeResult.Merge(
                effectResult,
                useSkillResult
            );
        }
    }
}
