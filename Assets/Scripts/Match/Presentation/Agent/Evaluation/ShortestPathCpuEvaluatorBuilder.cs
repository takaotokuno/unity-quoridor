namespace Quoridor
{
    /// <summary>
    /// shortest_path processor 用の CPU 評価関数を Preset パラメータから構築する Builder。
    /// </summary>
    public sealed class ShortestPathCpuEvaluatorBuilder : ICpuEvaluatorBuilder
    {
        public const int DefaultWinScore = 100000;
        public const int DefaultSelfDistanceWeight = 10;
        public const int DefaultOpponentDistanceWeight = 7;

        private const string WinScoreKey = "win_score";
        private const string SelfDistanceWeightKey = "self_distance_weight";
        private const string OpponentDistanceWeightKey = "opponent_distance_weight";

        private readonly DistanceCalculator _distanceCalculator;

        public ShortestPathCpuEvaluatorBuilder(DistanceCalculator distanceCalculator)
        {
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        }

        public CpuEvaluatorProcessorId ProcessorId => CpuEvaluatorProcessorId.ShortestPath;

        public ICpuEvaluator Build(CpuEvaluatorPreset preset)
        {
            Guard.ThrowIfNull(preset, nameof(preset));

            return new ShortestPathCpuEvaluator(
                _distanceCalculator,
                CreateSettings(preset)
            );
        }

        public ICpuEvaluator BuildDefault()
        {
            return new ShortestPathCpuEvaluator(
                _distanceCalculator,
                CreateDefaultSettings()
            );
        }

        public static ShortestPathCpuEvaluator.Settings CreateDefaultSettings()
        {
            return new ShortestPathCpuEvaluator.Settings(
                DefaultWinScore,
                DefaultSelfDistanceWeight,
                DefaultOpponentDistanceWeight
            );
        }

        private static ShortestPathCpuEvaluator.Settings CreateSettings(CpuEvaluatorPreset preset)
        {
            return new ShortestPathCpuEvaluator.Settings(
                GetInt(preset, WinScoreKey, DefaultWinScore),
                GetInt(preset, SelfDistanceWeightKey, DefaultSelfDistanceWeight),
                GetInt(preset, OpponentDistanceWeightKey, DefaultOpponentDistanceWeight)
            );
        }

        private static int GetInt(CpuEvaluatorPreset preset, string key, int defaultValue)
        {
            foreach (CpuEvaluatorParameterEntry parameter in preset.Parameters)
            {
                if (parameter == null || parameter.Key != key)
                    continue;

                return parameter.Value;
            }

            return defaultValue;
        }
    }
}
