namespace Quoridor
{
    /// <summary>
    /// 自分のゴール距離短縮と相手のゴール距離増加を重視する基本評価関数。
    /// </summary>
    public sealed class ShortestPathCpuEvaluator : ICpuEvaluator
    {
        private readonly DistanceCalculator _distanceCalculator;
        private readonly int _winScore;
        private readonly int _selfDistanceWeight;
        private readonly int _opponentDistanceWeight;

        public ShortestPathCpuEvaluator(
            DistanceCalculator distanceCalculator,
            Settings settings
        )
        {
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
            Settings validSettings = Guard.ThrowIfNull(settings, nameof(settings));
            _winScore = validSettings.WinScore;
            _selfDistanceWeight = validSettings.SelfDistanceWeight;
            _opponentDistanceWeight = validSettings.OpponentDistanceWeight;
        }

        public int Evaluate(MatchState state, PlayerId perspectivePlayerId)
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(perspectivePlayerId, nameof(perspectivePlayerId));

            var distances = _distanceCalculator.Calculate(state);
            int selfDistance = distances.GetDistance(perspectivePlayerId);
            int opponentDistance = distances.GetDistance(perspectivePlayerId.Opponent);

            if (selfDistance <= 0)
                return _winScore;

            if (opponentDistance <= 0)
                return -_winScore;

            return opponentDistance * _opponentDistanceWeight
                - selfDistance * _selfDistanceWeight;
        }

        /// <summary>
        /// ShortestPathCpuEvaluator の生成時に注入する調整済み設定。
        /// </summary>
        public sealed class Settings
        {
            public Settings(
                int winScore,
                int selfDistanceWeight,
                int opponentDistanceWeight
            )
            {
                WinScore = winScore;
                SelfDistanceWeight = selfDistanceWeight;
                OpponentDistanceWeight = opponentDistanceWeight;
            }

            public int WinScore { get; }
            public int SelfDistanceWeight { get; }
            public int OpponentDistanceWeight { get; }
        }
    }
}
