using System;

namespace Quoridor
{
    public sealed record SkillSlotId
    {
        public int Value { get; }
        public SkillSlotId(int value)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;   
        }

        public int ToIndex() => Value - 1;
    }
}
