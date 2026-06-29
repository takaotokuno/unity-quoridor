using System;

namespace Quoridor
{
    /// <summary>
    /// 最短経路差に加えて、進行度・壁の残数・可動性・中央寄せ・経路の迂回量を評価する CPU 評価関数。
    /// </summary>
    public sealed class StrategicCpuEvaluator : ICpuEvaluator
    {
        private readonly DistanceCalculator _distanceCalculator;
        private readonly Settings _settings;

        public StrategicCpuEvaluator(
            DistanceCalculator distanceCalculator,
            Settings settings
        )
        {
            _distanceCalculator = Guard.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
            _settings = Guard.ThrowIfNull(settings, nameof(settings));
        }

        public int Evaluate(MatchState state, PlayerId perspectivePlayerId)
        {
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(perspectivePlayerId, nameof(perspectivePlayerId));

            var distances = _distanceCalculator.Calculate(state);
            int selfDistance = distances.GetDistance(perspectivePlayerId);
            int opponentDistance = distances.GetDistance(perspectivePlayerId.Opponent);

            if (selfDistance <= 0)
                return _settings.WinScore;

            if (opponentDistance <= 0)
                return -_settings.WinScore;

            BoardState board = state.Board;
            Position selfPawn = PawnHelper.GetPawnPosition(board, perspectivePlayerId);
            Position opponentPawn = PawnHelper.GetPawnPosition(board, perspectivePlayerId.Opponent);

            int score = opponentDistance * _settings.OpponentDistanceWeight
                - selfDistance * _settings.SelfDistanceWeight;

            score += CalculateProgressScore(board, perspectivePlayerId, selfPawn, opponentPawn);
            score += CalculateWallReserveScore(state, perspectivePlayerId, selfDistance, opponentDistance);
            score += CalculateMobilityScore(board, selfPawn, opponentPawn);
            score += CalculateCenterScore(board, selfPawn, opponentPawn);
            score += CalculatePathSlackScore(board, perspectivePlayerId, selfPawn, opponentPawn, selfDistance, opponentDistance);
            score += CalculateThreatScore(selfDistance, opponentDistance);

            return Clamp(score, -_settings.WinScore, _settings.WinScore);
        }

        private int CalculateProgressScore(
            BoardState board,
            PlayerId perspectivePlayerId,
            Position selfPawn,
            Position opponentPawn
        )
        {
            int selfProgress = CalculateProgress(board, perspectivePlayerId, selfPawn);
            int opponentProgress = CalculateProgress(board, perspectivePlayerId.Opponent, opponentPawn);

            return (selfProgress - opponentProgress) * _settings.ProgressWeight;
        }

        private int CalculateWallReserveScore(
            MatchState state,
            PlayerId perspectivePlayerId,
            int selfDistance,
            int opponentDistance
        )
        {
            int selfWalls = GetRemainingWallUses(state, perspectivePlayerId);
            int opponentWalls = GetRemainingWallUses(state, perspectivePlayerId.Opponent);
            int baseScore = (selfWalls - opponentWalls) * _settings.WallReserveWeight;

            int urgency = Math.Max(0, _settings.WallUrgencyDistance - opponentDistance);
            int defenseBonus = selfWalls * urgency * _settings.WallUrgencyWeight;

            int winningTempo = Math.Max(0, opponentDistance - selfDistance);
            int preservedWallBonus = selfWalls * winningTempo * _settings.WinningWallReserveWeight;

            return baseScore + defenseBonus + preservedWallBonus;
        }

        private int CalculateMobilityScore(
            BoardState board,
            Position selfPawn,
            Position opponentPawn
        )
        {
            int selfMobility = CountOpenAdjacentTiles(board, selfPawn);
            int opponentMobility = CountOpenAdjacentTiles(board, opponentPawn);

            return (selfMobility - opponentMobility) * _settings.MobilityWeight;
        }

        private int CalculateCenterScore(
            BoardState board,
            Position selfPawn,
            Position opponentPawn
        )
        {
            int centerX = board.Grid.Width / 2;
            int selfCenterDistance = Math.Abs(selfPawn.X - centerX);
            int opponentCenterDistance = Math.Abs(opponentPawn.X - centerX);

            return (opponentCenterDistance - selfCenterDistance) * _settings.CenterControlWeight;
        }

        private int CalculatePathSlackScore(
            BoardState board,
            PlayerId perspectivePlayerId,
            Position selfPawn,
            Position opponentPawn,
            int selfDistance,
            int opponentDistance
        )
        {
            int selfDirectDistance = CalculateDirectGoalDistance(board, perspectivePlayerId, selfPawn);
            int opponentDirectDistance = CalculateDirectGoalDistance(board, perspectivePlayerId.Opponent, opponentPawn);
            int selfSlack = Math.Max(0, selfDistance - selfDirectDistance);
            int opponentSlack = Math.Max(0, opponentDistance - opponentDirectDistance);

            return (opponentSlack - selfSlack) * _settings.PathSlackWeight;
        }

        private int CalculateThreatScore(int selfDistance, int opponentDistance)
        {
            int score = 0;

            if (selfDistance <= _settings.FinishingDistance)
                score += (_settings.FinishingDistance - selfDistance + 1) * _settings.FinishingThreatWeight;

            if (opponentDistance <= _settings.FinishingDistance)
                score -= (_settings.FinishingDistance - opponentDistance + 1) * _settings.FinishingThreatWeight;

            return score;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        private static int CalculateProgress(
            BoardState board,
            PlayerId playerId,
            Position pawn
        )
        {
            return playerId.IsFirstPlayer
                ? pawn.Y
                : board.Grid.Height - 1 - pawn.Y;
        }

        private static int CalculateDirectGoalDistance(
            BoardState board,
            PlayerId playerId,
            Position pawn
        )
        {
            int distanceByCells = playerId.IsFirstPlayer
                ? board.Grid.Height - 1 - pawn.Y
                : pawn.Y;

            return distanceByCells / 2;
        }

        private static int CountOpenAdjacentTiles(BoardState board, Position pawn)
        {
            int count = 0;

            foreach (var direction in BoardDirections.FourDirections)
            {
                Position next = BoardGeometry.Add(pawn, direction);
                if (BoardGeometry.CanMoveOneTileIgnoringPawn(board, pawn, next))
                    count++;
            }

            return count;
        }

        private static int GetRemainingWallUses(MatchState state, PlayerId playerId)
        {
            PlayerState player = state.GetPlayer(playerId);
            if (!player.TryGetSkillBySlotId(BuiltInSkillSlotIds.PlaceWall, out SkillState wallSkill))
                return 0;

            return wallSkill.RemainingUses ?? 0;
        }

        /// <summary>
        /// StrategicCpuEvaluator の生成時に注入する調整済み設定。
        /// </summary>
        public sealed class Settings
        {
            public Settings(
                int winScore,
                int selfDistanceWeight,
                int opponentDistanceWeight,
                int progressWeight,
                int wallReserveWeight,
                int wallUrgencyDistance,
                int wallUrgencyWeight,
                int winningWallReserveWeight,
                int mobilityWeight,
                int centerControlWeight,
                int pathSlackWeight,
                int finishingDistance,
                int finishingThreatWeight
            )
            {
                WinScore = winScore;
                SelfDistanceWeight = selfDistanceWeight;
                OpponentDistanceWeight = opponentDistanceWeight;
                ProgressWeight = progressWeight;
                WallReserveWeight = wallReserveWeight;
                WallUrgencyDistance = wallUrgencyDistance;
                WallUrgencyWeight = wallUrgencyWeight;
                WinningWallReserveWeight = winningWallReserveWeight;
                MobilityWeight = mobilityWeight;
                CenterControlWeight = centerControlWeight;
                PathSlackWeight = pathSlackWeight;
                FinishingDistance = finishingDistance;
                FinishingThreatWeight = finishingThreatWeight;
            }

            public int WinScore { get; }
            public int SelfDistanceWeight { get; }
            public int OpponentDistanceWeight { get; }
            public int ProgressWeight { get; }
            public int WallReserveWeight { get; }
            public int WallUrgencyDistance { get; }
            public int WallUrgencyWeight { get; }
            public int WinningWallReserveWeight { get; }
            public int MobilityWeight { get; }
            public int CenterControlWeight { get; }
            public int PathSlackWeight { get; }
            public int FinishingDistance { get; }
            public int FinishingThreatWeight { get; }
        }
    }
}
