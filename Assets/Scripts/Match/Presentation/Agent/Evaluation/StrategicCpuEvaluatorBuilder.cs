namespace Quoridor
{
    /// <summary>
    /// strategic processor 用の CPU 評価関数を Preset パラメータから構築する Builder。
    /// </summary>
    public sealed class StrategicCpuEvaluatorBuilder : ICpuEvaluatorBuilder
    {
        public const int DefaultWinScore = 100000;
        public const int DefaultSelfDistanceWeight = 14;
        public const int DefaultOpponentDistanceWeight = 11;
        public const int DefaultProgressWeight = 2;
        public const int DefaultWallReserveWeight = 6;
        public const int DefaultWallUrgencyDistance = 3;
        public const int DefaultWallUrgencyWeight = 8;
        public const int DefaultWinningWallReserveWeight = 2;
        public const int DefaultMobilityWeight = 3;
        public const int DefaultCenterControlWeight = 1;
        public const int DefaultPathSlackWeight = 5;
        public const int DefaultFinishingDistance = 2;
        public const int DefaultFinishingThreatWeight = 30;

        private const string WinScoreKey = "win_score";
        private const string SelfDistanceWeightKey = "self_distance_weight";
        private const string OpponentDistanceWeightKey = "opponent_distance_weight";
        private const string ProgressWeightKey = "progress_weight";
        private const string WallReserveWeightKey = "wall_reserve_weight";
        private const string WallUrgencyDistanceKey = "wall_urgency_distance";
        private const string WallUrgencyWeightKey = "wall_urgency_weight";
        private const string WinningWallReserveWeightKey = "winning_wall_reserve_weight";
        private const string MobilityWeightKey = "mobility_weight";
        private const string CenterControlWeightKey = "center_control_weight";
        private const string PathSlackWeightKey = "path_slack_weight";
        private const string FinishingDistanceKey = "finishing_distance";
        private const string FinishingThreatWeightKey = "finishing_threat_weight";

        private readonly DistanceCalculator _distanceCalculator;

        public StrategicCpuEvaluatorBuilder(DistanceCalculator distanceCalculator)
        {
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        }

        public CpuEvaluatorProcessorId ProcessorId => CpuEvaluatorProcessorId.Strategic;

        public ICpuEvaluator Build(CpuEvaluatorPreset preset)
        {
            Guard.ThrowIfNull(preset, nameof(preset));

            return new StrategicCpuEvaluator(
                _distanceCalculator,
                CreateSettings(preset)
            );
        }

        public ICpuEvaluator BuildDefault()
        {
            return new StrategicCpuEvaluator(
                _distanceCalculator,
                CreateDefaultSettings()
            );
        }

        public static StrategicCpuEvaluator.Settings CreateDefaultSettings()
        {
            return new StrategicCpuEvaluator.Settings(
                DefaultWinScore,
                DefaultSelfDistanceWeight,
                DefaultOpponentDistanceWeight,
                DefaultProgressWeight,
                DefaultWallReserveWeight,
                DefaultWallUrgencyDistance,
                DefaultWallUrgencyWeight,
                DefaultWinningWallReserveWeight,
                DefaultMobilityWeight,
                DefaultCenterControlWeight,
                DefaultPathSlackWeight,
                DefaultFinishingDistance,
                DefaultFinishingThreatWeight
            );
        }

        private static StrategicCpuEvaluator.Settings CreateSettings(CpuEvaluatorPreset preset)
        {
            return new StrategicCpuEvaluator.Settings(
                GetInt(preset, WinScoreKey, DefaultWinScore),
                GetInt(preset, SelfDistanceWeightKey, DefaultSelfDistanceWeight),
                GetInt(preset, OpponentDistanceWeightKey, DefaultOpponentDistanceWeight),
                GetInt(preset, ProgressWeightKey, DefaultProgressWeight),
                GetInt(preset, WallReserveWeightKey, DefaultWallReserveWeight),
                GetInt(preset, WallUrgencyDistanceKey, DefaultWallUrgencyDistance),
                GetInt(preset, WallUrgencyWeightKey, DefaultWallUrgencyWeight),
                GetInt(preset, WinningWallReserveWeightKey, DefaultWinningWallReserveWeight),
                GetInt(preset, MobilityWeightKey, DefaultMobilityWeight),
                GetInt(preset, CenterControlWeightKey, DefaultCenterControlWeight),
                GetInt(preset, PathSlackWeightKey, DefaultPathSlackWeight),
                GetInt(preset, FinishingDistanceKey, DefaultFinishingDistance),
                GetInt(preset, FinishingThreatWeightKey, DefaultFinishingThreatWeight)
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
