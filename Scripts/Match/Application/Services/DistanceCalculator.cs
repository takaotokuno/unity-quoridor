namespace Quoridor
{
    public sealed class DistanceCalculator
    {
        private readonly Pathfinder _pathfinder;
        private readonly GoalResolver _goalResolver;

        public DistanceCalculator(
            Pathfinder pathfinder,
            GoalResolver goalResolver
        )
        {
            _pathfinder = pathfinder;
            _goalResolver = goalResolver;
        }

        public DistanceSnapshot Calculate(MatchState state)
        {
            var firstDistance = CalculateDistance(
                state.Board,
                PlayerId.FirstPlayer
            );

            var secondDistance = CalculateDistance(
                state.Board,
                PlayerId.SecondPlayer
            );

            return new DistanceSnapshot(
                firstDistance,
                secondDistance
            );
        }

        private int CalculateDistance(
            BoardState board,
            PlayerId playerId
        )
        {
            var start = PawnHelper.GetPawnPosition(board, playerId);

            if (_goalResolver.IsGoal(board, playerId, start))
            {
                return 0;
            }

            return _pathfinder.GetShortestDistanceToGoal(
                board,
                playerId
            );
        }
    }
}
