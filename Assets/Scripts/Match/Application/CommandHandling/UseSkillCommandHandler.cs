namespace Quoridor
{
    public sealed class UseSkillCommandHandler
    {
        private readonly MatchState _state;
        private readonly ISkillDefinitionRegistry _definitionRegistry;
        private readonly ISkillEffectResolver _effectResolver;
        private readonly ISkillLegalRuleRegistry _ruleRegistry;
        private readonly SkillAvailabilityValidator _skillAvailabilityValidator;

        public UseSkillCommandHandler(
            MatchState state,
            ISkillDefinitionRegistry definitionRegistry,
            ISkillEffectResolver effectResolver,
            ISkillLegalRuleRegistry ruleRegistry,
            SkillAvailabilityValidator skillAvailabilityValidator
        )
        {
            _state = Guard.ThrowIfNull(state, nameof(state));
            _definitionRegistry = Guard.ThrowIfNull(definitionRegistry, nameof(definitionRegistry));
            _effectResolver = Guard.ThrowIfNull(effectResolver, nameof(effectResolver));
            _ruleRegistry = Guard.ThrowIfNull(ruleRegistry, nameof(ruleRegistry));
            _skillAvailabilityValidator = Guard.ThrowIfNull(
                skillAvailabilityValidator,
                nameof(skillAvailabilityValidator)
            );
        }

        public CommandResult Handle(UseSkillCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            SkillAvailabilityResult availability = _skillAvailabilityValidator.Evaluate(
                _state,
                command.PlayerId,
                command.SkillSlotId
            );

            if (!availability.CanUse)
            {
                return CommandResultFactory.Reject(
                    command,
                    $"Skill Rejected: {availability.Message}"
                );
            }

            SkillState skill = _state
                .GetPlayer(command.PlayerId)
                .GetSkill(command.SkillSlotId);

            SkillDefinition definition = _definitionRegistry.Find(skill.SkillId);

            if (!IsLegal(command, definition))
            {
                return CommandResultFactory.Reject(
                    command,
                    "Skill Rejected: Illegal skill target"
                );
            }

            var result = _effectResolver.Resolve(command, definition);

            return new CommandResult(
                result.Events,
                consumeTurn: definition.ConsumeTurn
            );
        }

        private bool IsLegal(
            UseSkillCommand command,
            SkillDefinition definition
        )
        {
            var rule = _ruleRegistry.Find(definition.RuleId);

            var context = new SkillLegalContext(
                _state,
                command.PlayerId,
                definition,
                command.Target
            );

            return rule.IsLegal(context);
        }
    }
}
