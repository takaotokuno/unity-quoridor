namespace Quoridor
{
    /// <summary>
    /// CPU Evaluator Preset から実行時用の評価関数を構築する Builder。
    /// </summary>
    public interface ICpuEvaluatorBuilder
    {
        CpuEvaluatorProcessorId ProcessorId { get; }

        ICpuEvaluator Build(CpuEvaluatorPreset preset);

        ICpuEvaluator BuildDefault();
    }
}
