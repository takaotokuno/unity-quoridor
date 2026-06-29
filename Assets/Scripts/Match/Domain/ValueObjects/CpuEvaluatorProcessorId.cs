using System;

namespace Quoridor
{
    /// <summary>
    /// CPU Evaluator の評価ロジック種別を識別する ID。
    /// </summary>
    public sealed record CpuEvaluatorProcessorId
    {
        public string Value { get; }

        private CpuEvaluatorProcessorId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CpuEvaluatorProcessorId is required.", nameof(value));

            Value = value;
        }

        public static CpuEvaluatorProcessorId Of(string value) => new(value);

        public static readonly CpuEvaluatorProcessorId ShortestPath = new("shortest_path");
        public static readonly CpuEvaluatorProcessorId Strategic = new("strategic");

        public override string ToString() => Value;
    }
}
