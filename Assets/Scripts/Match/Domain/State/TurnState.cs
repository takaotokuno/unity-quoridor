using System;

namespace Quoridor
{
    public sealed class TurnState
    {
        public PlayerId CurrentPlayerId { get; private set; }
        public int CurrentTurn { get; private set; }

        public TurnState(PlayerSide startingSide = PlayerSide.First)
            : this(new PlayerId((int)startingSide), 1)
        {
        }

        private TurnState(PlayerId playerId, int currentTurn)
        {
            CurrentPlayerId = Guard.ThrowIfNull(playerId, nameof(playerId));

            if (currentTurn < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(currentTurn),
                    "Current turn must be greater than or equal to 1."
                );

            CurrentTurn = currentTurn;
        }

        public void NextTurn()
        {
            CurrentPlayerId = CurrentPlayerId.Opponent;
            CurrentTurn++;
        }

        public void PrevTurn()
        {
            if (CurrentTurn <= 1)
                throw new InvalidOperationException("Cannot go back before turn 1.");

            CurrentPlayerId = CurrentPlayerId.Opponent;
            CurrentTurn--;
        }

        public TurnState DeepCopy()
        {
            return new TurnState(CurrentPlayerId, CurrentTurn);
        }
    }
}
