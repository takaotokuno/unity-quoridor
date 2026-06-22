using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor
{
    public sealed class PlayerState
    {
        public PlayerId PlayerId { get; }
        public bool IsActive { get; }
        public IReadOnlyDictionary<SkillSlotId, SkillState> Skills => _skills;
        private readonly Dictionary<SkillSlotId, SkillState> _skills;
        public IReadOnlyList<StatusState> Statuses => _statuses;
        private readonly List<StatusState> _statuses;
        public PlayerRuntimeState Runtime { get; }

        public PlayerState(
            PlayerId playerId,
            bool isActive,
            Dictionary<SkillSlotId, SkillState> skills,
            List<StatusState> statuses,
            PlayerRuntimeState runtime
        )
        {
            PlayerId = Guard.ThrowIfNull(playerId, nameof(playerId));
            IsActive = isActive;
            _skills = Guard.ThrowIfNull(skills, nameof(skills));
            _statuses = Guard.ThrowIfNull(statuses, nameof(statuses));
            Runtime = Guard.ThrowIfNull(runtime, nameof(runtime));
        }

        public bool TryGetSkillBySlotId(
            SkillSlotId skillSlotId,
            out SkillState skill
        )
        {
            Guard.ThrowIfNull(skillSlotId, nameof(skillSlotId));

            return Skills.TryGetValue(skillSlotId, out skill);
        }

        public PlayerState DeepCopy()
        {
            Dictionary<SkillSlotId, SkillState> skills = new();

            foreach (var skill in Skills)
            {
                skills.Add(skill.Key, skill.Value.DeepCopy());
            }

            List<StatusState> statuses = new();

            foreach (var status in Statuses)
            {
                statuses.Add(status.DeepCopy());
            }

            return new PlayerState(
                PlayerId,
                IsActive,
                skills,
                statuses,
                Runtime.DeepCopy()
            );
        }

        public SkillState GetSkill(SkillSlotId skillSlotId)
        {
            Guard.ThrowIfNull(skillSlotId, nameof(skillSlotId));

            if (!Skills.TryGetValue(skillSlotId, out SkillState skill))
                throw new KeyNotFoundException("Skill slot was not found.");

            return skill;
        }

        public StateChangeResult UseSkill(SkillSlotId skillSlotId)
        {
            Guard.ThrowIfNull(skillSlotId, nameof(skillSlotId));

            SkillState skill = GetSkill(skillSlotId);

            if (!skill.CanUse())
                throw new InvalidOperationException("Skill cannot be used.");

            skill.Use();

            return StateChangeResult.Changed(
                new SkillUsedEvent(
                    PlayerId,
                    skill.SkillId,
                    skillSlotId
                )
            );
        }

        public bool HasStatus(StatusId statusId)
        {
            return Statuses.Any(status => status.StatusId == statusId);
        }

        public StateChangeResult AddStatus(StatusState status)
        {
            Guard.ThrowIfNull(status, nameof(status));

            _statuses.Add(status);

            return StateChangeResult.Changed(
                new StatusAddedEvent(
                    PlayerId,
                    status.StatusId
                )
            );
        }

        public StateChangeResult OnTurnStarted()
        {
            Advance();

            return RemoveStatuses();
        }

        private void Advance()
        {
            foreach (var skill in Skills)
            {
                skill.Value.Advance();
            }

            foreach (var status in Statuses)
            {
                status.Advance();
            }
        }

        private StateChangeResult RemoveStatuses()
        {
            List<StatusId> removedStatusIds = Statuses
                .Where(status => status.IsExpired)
                .Select(status => status.StatusId)
                .Distinct()
                .ToList();

            if (removedStatusIds.Count == 0)
                return StateChangeResult.NoChange();

            _statuses.RemoveAll(status => status.IsExpired);

            List<IMatchEvent> events = new();

            foreach (StatusId statusId in removedStatusIds)
            {
                bool stillExists = Statuses
                    .Any(status => status.StatusId == statusId);

                if (!stillExists)
                {
                    events.Add(
                        new StatusRemovedEvent(
                            PlayerId,
                            statusId
                        )
                    );
                }
            }

            return StateChangeResult.Changed(events);
        }
    }
}
