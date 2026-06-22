using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchMemento
    {
        private BoardState _board;
        private PlayerState[] _players;
        private TurnState _turn;
        public MatchPhase Phase { get; }

        public BoardState Board => _board.DeepCopy();
        public PlayerState[] Players => DeepCopyPlayers(_players);
        public TurnState Turn => _turn.DeepCopy();

        public MatchMemento(MatchState state)
        {
            _board = state.Board.DeepCopy();
            _players = DeepCopyPlayers(state.Players);
            _turn = state.Turn.DeepCopy();
            Phase = state.Phase;
        }

        private static PlayerState[] DeepCopyPlayers(IReadOnlyList<PlayerState> players)
        {
            var result = new PlayerState[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                result[i] = players[i].DeepCopy();
            }
            return result;
        }
    }
}
