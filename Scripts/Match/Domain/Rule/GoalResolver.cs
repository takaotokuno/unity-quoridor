namespace Quoridor
{
    public sealed class GoalResolver
    {
        public bool IsGoal(
            BoardState board,
            PlayerId playerId,
            Position position
        )
        {
            return IsGoal(board.Grid.Height, playerId, position);
        }

        public bool IsGoal(
            int boardHeight,
            PlayerId playerId,
            Position position
        )
        {
            if (playerId.IsFirstPlayer)
            {
                return position.Y == boardHeight - 1;
            }

            if (playerId.IsSecondPlayer)
            {
                return position.Y == 0;
            }

            return false;
        }

        public bool HasPlayerReachedGoal(
            BoardState board,
            PlayerId playerId
        )
        {
            var pawn = PawnHelper.GetPawnPosition(board, playerId);

            return IsGoal(board, playerId, pawn);
        }

        public PlayerId FindWinner(BoardState board)
        {
            if (HasPlayerReachedGoal(board, PlayerId.FirstPlayer))
            {
                return PlayerId.FirstPlayer;
            }

            if (HasPlayerReachedGoal(board, PlayerId.SecondPlayer))
            {
                return PlayerId.SecondPlayer;
            }

            return null;
        }
    }
}
