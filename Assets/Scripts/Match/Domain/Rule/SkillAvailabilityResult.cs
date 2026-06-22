using System;

namespace Quoridor
{
    public sealed class SkillAvailabilityResult
    {
        private static readonly SkillAvailabilityResult AvailableInstance = new(
            true,
            SkillAvailabilityRejectReason.None,
            "Skill is available"
        );

        public bool CanUse { get; }
        public SkillAvailabilityRejectReason RejectReason { get; }
        public string Message { get; }

        private SkillAvailabilityResult(
            bool canUse,
            SkillAvailabilityRejectReason rejectReason,
            string message
        )
        {
            CanUse = canUse;
            RejectReason = rejectReason;
            Message = Guard.ThrowIfNull(message, nameof(message));
        }

        public static SkillAvailabilityResult Available()
        {
            return AvailableInstance;
        }

        public static SkillAvailabilityResult Reject(
            SkillAvailabilityRejectReason rejectReason,
            string message
        )
        {
            if (rejectReason == SkillAvailabilityRejectReason.None)
                throw new ArgumentException("Reject reason must not be None.", nameof(rejectReason));

            return new SkillAvailabilityResult(false, rejectReason, message);
        }
    }
}
