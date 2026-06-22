namespace Quoridor
{
    public sealed class CommandHandlerFactory
    {
        private readonly ISkillDefinitionRegistry _skillDefinitionRegistry;
        private readonly ISkillEffectComposerRegistry _skillComposerRegistry;
        private readonly ISkillLegalRuleRegistry _ruleRegistry;
        private readonly SkillAvailabilityValidator _skillAvailabilityValidator;
        private readonly TurnAdvancer _turnAdvancer;

        public CommandHandlerFactory(
            ISkillDefinitionRegistry skillDefinitionRegistry,
            ISkillEffectComposerRegistry skillComposerRegistry,
            ISkillLegalRuleRegistry ruleRegistry,
            SkillAvailabilityValidator skillAvailabilityValidator,
            TurnAdvancer turnAdvancer
        )
        {
            _skillDefinitionRegistry = Guard.ThrowIfNull(skillDefinitionRegistry, nameof(skillDefinitionRegistry));
            _skillComposerRegistry = Guard.ThrowIfNull(skillComposerRegistry, nameof(skillComposerRegistry));
            _ruleRegistry = Guard.ThrowIfNull(ruleRegistry, nameof(ruleRegistry));
            _skillAvailabilityValidator = Guard.ThrowIfNull(
                skillAvailabilityValidator,
                nameof(skillAvailabilityValidator)
            );
            _turnAdvancer = Guard.ThrowIfNull(turnAdvancer, nameof(turnAdvancer));
        }

        public CommandVisitor Create(MatchState state)
        {
            Guard.ThrowIfNull(state, nameof(state));

            var effectResolver = new SkillEffectResolver(
                state,
                _skillComposerRegistry
            );

            var useSkillCommandHandler = new UseSkillCommandHandler(
                state,
                _skillDefinitionRegistry,
                effectResolver,
                _ruleRegistry,
                _skillAvailabilityValidator
            );

            var matchControlCommandHandler =
                new MatchControlCommandHandler(
                    state,
                    _turnAdvancer
                );

            return new CommandVisitor(
                state,
                useSkillCommandHandler,
                matchControlCommandHandler
            );
        }
    }
}
