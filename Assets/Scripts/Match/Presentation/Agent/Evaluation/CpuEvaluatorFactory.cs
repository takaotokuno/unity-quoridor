using System;
using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// CPU Agent 生成時に Evaluator Preset に対応する Builder を選び、評価関数を生成する Factory。
    /// </summary>
    public sealed class CpuEvaluatorFactory
    {
        private readonly Dictionary<CpuEvaluatorProcessorId, ICpuEvaluatorBuilder> _builders = new();

        public CpuEvaluatorFactory(IEnumerable<ICpuEvaluatorBuilder> builders)
        {
            Guard.ThrowIfNull(builders, nameof(builders));

            foreach (ICpuEvaluatorBuilder builder in builders)
            {
                _builders.Add(builder.ProcessorId, builder);
            }
        }

        public ICpuEvaluator Create(CpuEvaluatorPreset preset)
        {
            Guard.ThrowIfNull(preset, nameof(preset));

            return GetBuilder(preset.ProcessorId).Build(preset);
        }

        public ICpuEvaluator CreateDefault(CpuEvaluatorProcessorId processorId)
        {
            Guard.ThrowIfNull(processorId, nameof(processorId));

            return GetBuilder(processorId).BuildDefault();
        }

        private ICpuEvaluatorBuilder GetBuilder(CpuEvaluatorProcessorId processorId)
        {
            if (_builders.TryGetValue(processorId, out ICpuEvaluatorBuilder builder))
                return builder;

            throw new InvalidOperationException(
                $"CpuEvaluatorBuilder not found. ProcessorId: {processorId}"
            );
        }
    }
}
