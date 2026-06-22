using System;

namespace Quoridor{
    public sealed record SkillLegalRuleId
    {
        public string Value { get; }

        private SkillLegalRuleId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SkillId is required.", nameof(value));

            Value = value;
        }

        public static SkillLegalRuleId Of(string value) => new(value);
        public override string ToString() => Value;

        public static readonly SkillLegalRuleId AnyTile = new("any_tile");
        public static readonly SkillLegalRuleId AnyWall = new("any_wall");

    }
}
