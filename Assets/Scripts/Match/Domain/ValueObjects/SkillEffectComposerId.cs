
using System;

namespace Quoridor{
    public sealed record SkillEffectComposerId
    {
        public string Value { get; }

        private SkillEffectComposerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SkillId is required.", nameof(value));

            Value = value;
        }

        public static SkillEffectComposerId Of(string value) => new(value);
        public override string ToString() => Value;
    }
}