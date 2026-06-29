using System;

namespace Quoridor
{
    /// <summary>
    /// CPU 探索に使う思考時間上限をミリ秒単位で表す Value Object。
    /// </summary>
    public sealed record CpuSearchTimeLimit
    {
        public const int DefaultValue = 1000;
        private const int MinimumValue = 1;

        public static CpuSearchTimeLimit Default { get; } = new CpuSearchTimeLimit(
            DefaultValue
        );

        public int Value { get; }

        private CpuSearchTimeLimit(int valueMilliseconds)
        {
            Value = Math.Max(MinimumValue, valueMilliseconds);
        }

        public static CpuSearchTimeLimit OfMilliseconds(int valueMilliseconds) => new(valueMilliseconds);

        public override string ToString() => Value.ToString();
    }
}
