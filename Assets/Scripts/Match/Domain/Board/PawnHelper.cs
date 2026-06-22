namespace Quoridor
{
    public static class PawnHelper
    {
        public static Position GetPawnPosition(
            BoardState board,
            PlayerId playerId
        )
        {
            return board.Pawns[playerId.ToIndex()];
        }

        public static Position GetOpponentPawnPosition(
            BoardState board,
            PlayerId playerId
        )
        {
            var opponentId = playerId.Opponent;
            return board.Pawns[opponentId.ToIndex()];
        }
    }
}