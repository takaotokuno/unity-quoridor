using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor
{
    public sealed class MatchState
    {
        public BoardState Board { get; private set; }
        private PlayerState[] _players;
        public IReadOnlyList<PlayerState> Players => _players;
        public TurnState Turn { get; private set; }
        public MatchPhase Phase { get; private set; }

        public PlayerId CurrentPlayerId => Turn.CurrentPlayerId;
        public PlayerState CurrentPlayer => _players[Turn.CurrentPlayerId.ToIndex()];
        public PlayerState GetPlayer(PlayerId playerId) => _players[playerId.ToIndex()];
        public bool IsCurrentPlayer(PlayerId playerId) => CurrentPlayerId == playerId;
        public int CurrentTurn => Turn.CurrentTurn;
        public bool IsInProgress => Phase == MatchPhase.InProgress;

        public MatchState(
            BoardState board,
            IReadOnlyList<PlayerState> players,
            TurnState turn,
            MatchPhase phase = MatchPhase.NotStarted
        )
        {
            Board = Guard.ThrowIfNull(board, nameof(board));
            IReadOnlyList<PlayerState> validPlayers = Guard.ThrowIfNull(players, nameof(players));

            if (validPlayers.Count != 2)
                throw new ArgumentException("Players must contain exactly 2 elements.", nameof(players));

            for (int i = 0; i < validPlayers.Count; i++)
            {
                if (validPlayers[i] == null)
                    throw new ArgumentException($"Player at index {i} is null.", nameof(players));
            }

            _players = validPlayers.ToArray();
            Turn = Guard.ThrowIfNull(turn, nameof(turn));
            Phase = phase;
        }

        public MatchMemento Capture()
        {
            return new MatchMemento(this);
        }

        public StateChangeResult Restore(MatchMemento memento)
        {
            Guard.ThrowIfNull(memento, nameof(memento));

            Board = memento.Board;
            _players = memento.Players;
            Turn = memento.Turn;
            Phase = memento.Phase;

            return StateChangeResult.Changed(
                new StateRestoredEvent(Capture())
            );
        }

        public StateChangeResult Start()
        {
            if (Phase != MatchPhase.NotStarted)
                throw new InvalidOperationException("Match can only start from NotStarted.");

            Phase = MatchPhase.InProgress;

            return StateChangeResult.Changed(
                new MatchStartedEvent(CurrentPlayerId)
            );
        }

        public StateChangeResult Pause()
        {
            if (Phase != MatchPhase.InProgress)
                throw new InvalidOperationException("Only an in-progress match can be paused.");

            Phase = MatchPhase.Paused;

            return StateChangeResult.NoChange();
        }

        public StateChangeResult Resume()
        {
            if (Phase != MatchPhase.Paused)
                throw new InvalidOperationException("Only a paused match can be resumed.");

            Phase = MatchPhase.InProgress;

            return StateChangeResult.NoChange();
        }

        public StateChangeResult Finish()
        {
            if (Phase is not MatchPhase.InProgress and not MatchPhase.Paused)
                throw new InvalidOperationException("Only an active match can be finished.");

            Phase = MatchPhase.Finished;

            return StateChangeResult.Changed(
                new MatchFinishedEvent()
            );
        }

        public void EnsureInProgress()
        {
            if (Phase != MatchPhase.InProgress)
                throw new InvalidOperationException("Match is not in progress.");
        }
    }
}
