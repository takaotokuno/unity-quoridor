using System;

namespace Quoridor
{
    /// <summary>
    /// 調整済み CPU Evaluator 定義を識別する ID。
    /// </summary>
    public sealed record CpuEvaluatorId
    {
        public string Value { get; }

        private CpuEvaluatorId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CpuEvaluatorId is required.", nameof(value));

            Value = value;
        }

        public static CpuEvaluatorId Of(string value) => new(value);

        public override string ToString() => Value;
    }
}
