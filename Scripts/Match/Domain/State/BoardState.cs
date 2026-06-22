using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class BoardState
    {
        private readonly int[,] _grid;
        private readonly IntGridView _gridView;
        private readonly Position[] _pawns;

        public IReadOnlyList<Position> Pawns => _pawns;
        public IReadOnlyIntGrid Grid => _gridView;

        public BoardState(
            int[,] grid,
            Position[] pawns
        )
        {
            Guard.ThrowIfNull(grid, nameof(grid));

            Guard.ThrowIfNull(pawns, nameof(pawns));

            if (pawns.Length != 2)
                throw new ArgumentException(
                    "Pawns must contain exactly 2 elements.",
                    nameof(pawns)
                );

            _grid = (int[,])grid.Clone();
            _gridView = new IntGridView(_grid);
            _pawns = (Position[])pawns.Clone();
        }

        public Position GetPawn(PlayerId playerId)
        {
            Guard.ThrowIfNull(playerId, nameof(playerId));

            return _pawns[playerId.ToIndex()];
        }

        public BoardState DeepCopy()
        {
            return new BoardState(
                (int[,])_grid.Clone(),
                (Position[])_pawns.Clone()
            );
        }

        public StateChangeResult MovePawn(PlayerId playerId, Position to)
        {
            Guard.ThrowIfNull(playerId, nameof(playerId));

            if (!BoardGeometry.IsInside(this, to))
                throw new ArgumentOutOfRangeException(nameof(to));

            _pawns[playerId.ToIndex()] = to;

            return StateChangeResult.Changed(
                new PawnMovedEvent(playerId, to)
            );
        }

        public StateChangeResult PlaceWall(IReadOnlyList<Position> walls)
        {
            Guard.ThrowIfNull(walls, nameof(walls));

            ValidateWallPositionsCanBePlaced(walls);

            foreach (Position wall in walls)
            {
                _grid[wall.Y, wall.X] = 1;
            }

            return StateChangeResult.Changed(
                new WallPlacedEvent(new List<Position>(walls))
            );
        }

        public StateChangeResult RemoveWall(IReadOnlyList<Position> walls)
        {
            Guard.ThrowIfNull(walls, nameof(walls));

            ValidateWallPositionsCanBeRemoved(walls);

            foreach (Position wall in walls)
            {
                _grid[wall.Y, wall.X] = 0;
            }

            return StateChangeResult.Changed(
                new WallRemovedEvent(new List<Position>(walls))
            );
        }

        private void ValidateWallPositionsCanBePlaced(
            IReadOnlyList<Position> walls
        )
        {
            for (int i = 0; i < walls.Count; i++)
            {
                Position wall = walls[i];

                ValidateWallPosition(wall);

                if (BoardGeometry.IsWall(this, wall))
                    throw new InvalidOperationException("Wall already exists.");
            }
        }

        private void ValidateWallPositionsCanBeRemoved(
            IReadOnlyList<Position> walls
        )
        {
            for (int i = 0; i < walls.Count; i++)
            {
                Position wall = walls[i];

                ValidateWallPosition(wall);

                if (BoardGeometry.IsEmpty(this, wall))
                    throw new InvalidOperationException("Wall doesn't exist.");
            }
        }

        private void ValidateWallPosition(Position wall)
        {
            if (!BoardGeometry.IsInside(this, wall))
                throw new ArgumentOutOfRangeException(nameof(wall));

            if (!BoardGeometry.IsWallPosition(wall))
                throw new ArgumentOutOfRangeException(nameof(wall));
        }
    }
}
