using System;

namespace Quoridor
{
    public sealed class ApplyStatusEffectComposer : ISkillEffectComposer
    {
        private readonly StatusApplicator _statusApplicator;

        public SkillEffectComposerId ComposerId => SkillEffectComposerId.Of("apply_status");

        public ApplyStatusEffectComposer(StatusApplicator statusApplicator)
        {
            _statusApplicator = Guard.ThrowIfNull(statusApplicator, nameof(statusApplicator));
        }

        public StateChangeResult Compose(SkillEffectContext context)
        {
            Guard.ThrowIfNull(context, nameof(context));

            StatusId statusId = GetStatusId(context);
            PlayerState targetPlayer = ResolveTargetPlayer(context);

            int remainingTurns = context.Definition.GetInt(
                StatusParameterKeys.RemainingTurns
            );

            int coolDownTurns = context.Definition.GetInt(
                StatusParameterKeys.CoolDownRemaining
            );

            StatusState status = new StatusState(
                statusId,
                remainingTurns,
                coolDownTurns,
                coolDownTurns
            );

            return _statusApplicator.Apply(
                targetPlayer,
                status
            );
        }

        private static StatusId GetStatusId(SkillEffectContext context)
        {
            int rawStatusId = context.Definition.GetInt(
                SkillParameterKeys.StatusId
            );

            if (!Enum.IsDefined(typeof(StatusId), rawStatusId))
            {
                throw new InvalidOperationException(
                    $"Invalid StatusId parameter: {rawStatusId}"
                );
            }

            return (StatusId)rawStatusId;
        }

        private static PlayerState ResolveTargetPlayer(SkillEffectContext context)
        {
            int rawPolicy = context.Definition.GetInt(
                SkillParameterKeys.TargetPlayer
            );

            if (!Enum.IsDefined(typeof(SkillTargetPlayerPolicy), rawPolicy))
            {
                throw new InvalidOperationException(
                    $"Invalid target player policy: {rawPolicy}"
                );
            }

            SkillTargetPlayerPolicy policy = (SkillTargetPlayerPolicy)rawPolicy;

            switch (policy)
            {
                case SkillTargetPlayerPolicy.Self:
                    return context.State.GetPlayer(context.PlayerId);

                case SkillTargetPlayerPolicy.Opponent:
                    return context.State.GetPlayer(context.PlayerId.Opponent);

                default:
                    throw new InvalidOperationException(
                        $"Unsupported target player policy: {policy}"
                    );
            }
        }
    }
}
