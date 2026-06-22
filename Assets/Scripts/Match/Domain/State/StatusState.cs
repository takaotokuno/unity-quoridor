using System;

namespace Quoridor
{
    public class StatusState
    {
        public StatusId StatusId { get; }
        public int? RemainingTurns { get; private set; }
        public int CoolDownTurns { get; }
        public int CoolDownRemaining { get; private set; }

        public bool IsExpired => RemainingTurns.HasValue && RemainingTurns.Value <= 0;
        public bool IsReady => !IsExpired && CoolDownRemaining <= 0;

        public StatusState(
            StatusId statusId,
            int? remainingTurns,
            int coolDownTurns,
            int coolDownRemaining
        )
        {
            if (remainingTurns < 1)
                throw new ArgumentOutOfRangeException(nameof(remainingTurns), "RemainingTurns must be null or 1 or greater");
            
            if (coolDownTurns < 0)
                throw new ArgumentOutOfRangeException(nameof(coolDownTurns), "CoolDownTurns must be 0 or greater");

            if (coolDownRemaining < 0)
                throw new ArgumentOutOfRangeException(nameof(coolDownRemaining), "CoolDownRemaining must be 0 or greater");

            if (coolDownRemaining > coolDownTurns)
                throw new ArgumentOutOfRangeException(nameof(coolDownRemaining), "CoolDownRemaining must not exceed CoolDownTurns");

            StatusId = statusId;
            RemainingTurns = remainingTurns;
            CoolDownTurns = coolDownTurns;
            CoolDownRemaining = coolDownRemaining;
        }

        public StatusState DeepCopy()
        {
            return new StatusState
            (
                this.StatusId,
                this.RemainingTurns,
                this.CoolDownTurns,
                this.CoolDownRemaining
            );
        }

        public void Advance()
        {
            if (CoolDownRemaining > 0)
            {
                CoolDownRemaining --;   
            }

            if (RemainingTurns > 0)
            {
                RemainingTurns --;   
            }
        }

        public void RefreshRemaining(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be 1 or greater");

            RemainingTurns = amount;
        }
    }
}
