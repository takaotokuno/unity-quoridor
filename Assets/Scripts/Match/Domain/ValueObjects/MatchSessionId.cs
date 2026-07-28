using System;

namespace Quoridor
{
    /// <summary>
    /// Identifies a single match session.
    /// </summary>
    public sealed record MatchSessionId
    {
        public int Value { get; }

        public MatchSessionId(int value)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
        }

        public MatchSessionId Next() => new(checked(Value + 1));

        public override string ToString() => Value.ToString();
    }
}
