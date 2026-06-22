using System.Linq;

namespace Quoridor
{
    public sealed class CheckmateResolver
    {
        public bool IsCheckmate(
            MatchState match,
            PlayerId playerId,
            PlayerId nextPlayerId,
            DistanceSnapshot distances
        )
        {
            var player = match.GetPlayer(playerId);

            if (!HasUsedAllNonMoveSkills(player))
            {
                return false;
            }

            var opponentId = playerId.Opponent;

            var playerDistance = distances.GetDistance(playerId);
            var opponentDistance = distances.GetDistance(opponentId);

            if (playerDistance > opponentDistance)
            {
                return true;
            }

            if (playerDistance == opponentDistance
                && !playerId.Equals(nextPlayerId))
            {
                return true;
            }

            return false;
        }

        public PlayerId FindWinner(
            MatchState match,
            DistanceSnapshot distances
        )
        {
            // Checkmate is resolved before turn advancement.
            // Therefore CurrentPlayerId is the player who just acted, and the next player is its opponent.
            var nextPlayerId = match.CurrentPlayerId.Opponent;

            var firstPlayerCheckmated =
                IsCheckmate(
                    match,
                    PlayerId.FirstPlayer,
                    nextPlayerId,
                    distances
                );

            var secondPlayerCheckmated =
                IsCheckmate(
                    match,
                    PlayerId.SecondPlayer,
                    nextPlayerId,
                    distances
                );

            if (firstPlayerCheckmated && !secondPlayerCheckmated)
            {
                return PlayerId.SecondPlayer;
            }

            if (secondPlayerCheckmated && !firstPlayerCheckmated)
            {
                return PlayerId.FirstPlayer;
            }

            return null;
        }

        private bool HasUsedAllNonMoveSkills(PlayerState player)
        {
            return player.Skills
                .Where(x => x.Key != BuiltInSkillSlotIds.MovePawn)
                .All(x => x.Value.RemainingUses <= 0);
        }
    }
}
