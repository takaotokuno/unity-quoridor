namespace Quoridor
{
    /// <summary>
    /// CPU エージェントの Strategy 種別、探索時間上限、評価プリセット参照をまとめた設定。
    /// </summary>
    public sealed class CpuAgentOptions
    {
        public const int DefaultSearchTimeLimitMilliseconds = CpuSearchTimeLimit.DefaultValue;

        public static CpuAgentOptions Default { get; } = new CpuAgentOptions(
            CpuAgentStrategyKind.RandomLegal,
            CpuSearchTimeLimit.Default,
            null
        );

        public CpuAgentStrategyKind StrategyKind { get; }
        public CpuSearchTimeLimit SearchTimeLimit { get; }
        public CpuEvaluatorPreset EvaluatorPreset { get; }

        public CpuAgentOptions(
            CpuAgentStrategyKind strategyKind,
            int searchTimeLimitMilliseconds,
            CpuEvaluatorPreset evaluatorPreset
        )
            : this(
                strategyKind,
                CpuSearchTimeLimit.OfMilliseconds(searchTimeLimitMilliseconds),
                evaluatorPreset
            )
        {
        }

        public CpuAgentOptions(
            CpuAgentStrategyKind strategyKind,
            CpuSearchTimeLimit searchTimeLimit,
            CpuEvaluatorPreset evaluatorPreset
        )
        {
            StrategyKind = strategyKind;
            SearchTimeLimit = searchTimeLimit ?? CpuSearchTimeLimit.Default;
            EvaluatorPreset = evaluatorPreset;
        }
    }
}
