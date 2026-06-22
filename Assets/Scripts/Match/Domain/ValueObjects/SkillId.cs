using System;

namespace Quoridor{
    public sealed record SkillId
    {
        public string Value { get; }

        private SkillId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SkillId is required.", nameof(value));

            Value = value;
        }

        public static SkillId Of(string value) => new(value);

        public static readonly SkillId NormalMovePawn = new("normal_move_pawn");
        public static readonly SkillId NormalPlaceWall = new("normal_place_wall");

        public override string ToString() => Value;
    }
}
