using System;
using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// αβ探索用の最小局面表現。
    /// 探索開始時に MatchState から一度だけ生成し、探索中は Apply/Undo で in-place 更新する。
    /// </summary>
    public sealed class SearchState
    {
        private readonly int[,] _grid;
        private readonly Position[] _pawns;

        public int Width { get; }
        public int Height { get; }
        public PlayerId CurrentPlayerId { get; private set; }
        public int CurrentTurn { get; private set; }

        public SearchState(MatchState state)
        {
            Guard.ThrowIfNull(state, nameof(state));

            var board = state.Board;
            Width = board.Grid.Width;
            Height = board.Grid.Height;
            _grid = new int[Height, Width];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    _grid[y, x] = board.Grid.Get(x, y);
                }
            }

            _pawns = new Position[board.Pawns.Count];
            for (var i = 0; i < board.Pawns.Count; i++)
            {
                _pawns[i] = board.Pawns[i];
            }

            CurrentPlayerId = state.CurrentPlayerId;
            CurrentTurn = state.CurrentTurn;
        }

        public Position GetPawn(PlayerId playerId)
        {
            Guard.ThrowIfNull(playerId, nameof(playerId));

            return _pawns[playerId.ToIndex()];
        }

        public bool IsWall(Position position)
        {
            EnsureInside(position, nameof(position));

            return _grid[position.Y, position.X] == 1;
        }

        public SearchUndo Apply(SearchMove move)
        {
            EnsureCurrentPlayer(move.PlayerId);

            switch (move.Kind)
            {
                case SearchMoveKind.MovePawn:
                    return ApplyMovePawn(move);

                case SearchMoveKind.PlaceWall:
                    return ApplyPlaceWall(move);

                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move.Kind, "Unknown search move kind.");
            }
        }

        public void Undo(SearchMove move, SearchUndo undo)
        {
            switch (move.Kind)
            {
                case SearchMoveKind.MovePawn:
                    _pawns[move.PlayerId.ToIndex()] = undo.PreviousPawn;
                    break;

                case SearchMoveKind.PlaceWall:
                    RemoveWalls(undo.WallCells);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move.Kind, "Unknown search move kind.");
            }

            CurrentPlayerId = undo.PreviousCurrentPlayerId;
            CurrentTurn = undo.PreviousTurn;
        }

        private SearchUndo ApplyMovePawn(SearchMove move)
        {
            EnsureInside(move.Target, nameof(move.Target));
            if (!BoardGeometry.IsTilePosition(move.Target))
            {
                throw new ArgumentException("Move target must be a tile position.", nameof(move));
            }

            var previousPawn = _pawns[move.PlayerId.ToIndex()];
            var undo = SearchUndo.ForMovePawn(
                move.PlayerId,
                CurrentPlayerId,
                CurrentTurn,
                previousPawn
            );

            _pawns[move.PlayerId.ToIndex()] = move.Target;
            AdvanceTurn();

            return undo;
        }

        private SearchUndo ApplyPlaceWall(SearchMove move)
        {
            EnsureWallCellsCanBePlaced(move.WallCells);

            var undo = SearchUndo.ForPlaceWall(
                move.PlayerId,
                CurrentPlayerId,
                CurrentTurn,
                move.WallCells
            );
            foreach (var wall in move.WallCells)
            {
                _grid[wall.Y, wall.X] = 1;
            }

            AdvanceTurn();

            return undo;
        }

        private void RemoveWalls(IReadOnlyList<Position> walls)
        {
            foreach (var wall in walls)
            {
                EnsureInside(wall, nameof(walls));
                _grid[wall.Y, wall.X] = 0;
            }
        }

        private void EnsureWallCellsCanBePlaced(IReadOnlyList<Position> walls)
        {
            if (walls.Count == 0)
            {
                throw new ArgumentException("Wall cells must not be empty.", nameof(walls));
            }

            foreach (var wall in walls)
            {
                EnsureInside(wall, nameof(walls));

                if (!BoardGeometry.IsWallPosition(wall))
                {
                    throw new ArgumentException("Wall cell must be a wall position.", nameof(walls));
                }

                if (_grid[wall.Y, wall.X] != 0)
                {
                    throw new InvalidOperationException("Wall already exists.");
                }
            }
        }

        private void EnsureCurrentPlayer(PlayerId playerId)
        {
            Guard.ThrowIfNull(playerId, nameof(playerId));

            if (playerId != CurrentPlayerId)
            {
                throw new InvalidOperationException("Search move player must match current player.");
            }
        }

        private void EnsureInside(Position position, string paramName)
        {
            if (!BoardGeometry.IsInside(position, Width, Height))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        private void AdvanceTurn()
        {
            CurrentPlayerId = CurrentPlayerId.Opponent;
            CurrentTurn++;
        }
    }
}
