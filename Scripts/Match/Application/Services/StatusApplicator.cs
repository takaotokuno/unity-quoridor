using System;
using System.Linq;

namespace Quoridor
{
    public sealed class StatusApplicator
    {
        private readonly IStatusDefinitionRegistry _statusDefinitionRegistry;

        public StatusApplicator(
            IStatusDefinitionRegistry statusDefinitionRegistry
        )
        {
            _statusDefinitionRegistry = Guard.ThrowIfNull(statusDefinitionRegistry, nameof(statusDefinitionRegistry));
        }

        public StateChangeResult Apply(
            PlayerState player,
            StatusState newStatus
        )
        {
            Guard.ThrowIfNull(player, nameof(player));

            Guard.ThrowIfNull(newStatus, nameof(newStatus));

            StatusDefinition definition =
                _statusDefinitionRegistry.Find(newStatus.StatusId);

            switch (definition.ReapplyPolicy)
            {
                case StatusReapplyPolicy.Ignore:
                    return ApplyUnique(player, newStatus);

                case StatusReapplyPolicy.Refresh:
                    return ApplyRefresh(player, newStatus);

                case StatusReapplyPolicy.Stack:
                    return ApplyStack(player, newStatus);

                default:
                    throw new InvalidOperationException(
                        $"Unsupported status stack policy: {definition.ReapplyPolicy}"
                    );
            }
        }

        private static StateChangeResult ApplyUnique(
            PlayerState player,
            StatusState newStatus
        )
        {
            if (player.HasStatus(newStatus.StatusId))
                return StateChangeResult.NoChange();

            return player.AddStatus(newStatus);
        }

        private static StateChangeResult ApplyRefresh(
            PlayerState player,
            StatusState newStatus
        )
        {
            if (!newStatus.RemainingTurns.HasValue)
                throw new ArgumentOutOfRangeException(nameof(newStatus), "Refresh Status must has Remaining Turns Value.");

            StatusState existing = player.Statuses
                .FirstOrDefault(status => status.StatusId == newStatus.StatusId);

            if (existing == null)
            {
                return player.AddStatus(newStatus);
            }

            existing.RefreshRemaining(newStatus.RemainingTurns.Value);

            return StateChangeResult.NoChange();
        }

        private static StateChangeResult ApplyStack(
            PlayerState player,
            StatusState newStatus)
        {
            return player.AddStatus(newStatus);
        }
    }
}
