using System;

namespace Quoridor
{
    public sealed class SkillState
    {
        public SkillId SkillId { get; }
        public int? RemainingUses { get; private set; }
        public int CoolDownTurns { get; }
        public int CoolDownRemaining { get; private set; }
        public int? Charge { get; private set; }

        public SkillState(
            SkillId skillId,
            int? remainingUses,
            int coolDownTurns,
            int coolDownRemaining,
            int? charge
        )
        {
            if (remainingUses < 0)
                throw new ArgumentOutOfRangeException(nameof(remainingUses), "RemainingUses must be null or 0 or greater.");

            if (coolDownTurns < 0)
                throw new ArgumentOutOfRangeException(nameof(coolDownTurns), "CoolDownTurns must be 0 or greater.");

            if (coolDownRemaining < 0)
                throw new ArgumentOutOfRangeException(nameof(coolDownRemaining), "CoolDownRemaining must be 0 or greater.");

            if (coolDownRemaining > coolDownTurns)
                throw new ArgumentOutOfRangeException(nameof(coolDownRemaining), "CoolDownRemaining must not exceed CoolDownTurns.");

            if (charge < 0)
                throw new ArgumentOutOfRangeException(nameof(charge), "Charge must be null or 0 or greater.");

            SkillId = skillId;
            RemainingUses = remainingUses;
            CoolDownTurns = coolDownTurns;
            CoolDownRemaining = coolDownRemaining;
            Charge = charge;
        }

        public SkillState DeepCopy()
        {
            return new SkillState
            (
                SkillId,
                RemainingUses,
                CoolDownTurns,
                CoolDownRemaining,
                Charge
            );
        }

        public bool CanUse()
        {
            if (RemainingUses.HasValue && RemainingUses.Value <= 0)
                return false;

            if (CoolDownRemaining > 0)
                return false;

            return true;
        }

        public void Use()
        {
            if (RemainingUses.HasValue && RemainingUses.Value <= 0)
                throw new InvalidOperationException("This skill has no remaining uses.");

            if (CoolDownRemaining > 0)
                throw new InvalidOperationException("This skill is cooling down.");

            if (RemainingUses.HasValue)
            {
                RemainingUses--;
            }

            CoolDownRemaining = CoolDownTurns;

            if (Charge.HasValue)
            {
                Charge = 0;
            }
        }

        public void Advance()
        {
            if (CoolDownRemaining > 0)
            {
                CoolDownRemaining --;
            }

            if (Charge.HasValue)
            {
                Charge ++;        
            }
        }

        public void Consume(int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException("amount", "Amount must be 0 or greater");

            if (RemainingUses.HasValue)
            {
                RemainingUses = Math.Max(RemainingUses.Value-amount, 0);
            }
        }

        public void Recover(int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException("amount", "Amount must be 0 or greater");

            RemainingUses += amount;
        }

        public void ResetCharge()
        {
            if (!Charge.HasValue)
                throw new InvalidOperationException("This skill does not use charge.");

            Charge = 0;
        }
    }
}
