using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed record PlayerId
    {
        public int Value { get; }
        public PlayerId(int value)
        {
            if (value is not 1 and not 2)
                throw new ArgumentOutOfRangeException(nameof(value), "PlayerId must be 1 or 2.");
            
            Value = value;
        }

        public static PlayerId FirstPlayer { get; } = new(1);
        public static PlayerId SecondPlayer { get; } = new(2);

        public bool IsFirstPlayer => Value == 1;
        public bool IsSecondPlayer => Value == 2;

        public static IReadOnlyList<PlayerId> All = new[]
        {
            FirstPlayer,
            SecondPlayer
        };

        public PlayerId Opponent => Value == 1 ? SecondPlayer : FirstPlayer;
        public int ToIndex() => Value - 1;

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
