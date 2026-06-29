using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// SearchState.Apply() で変更した内容を元に戻すための最小差分。
    /// </summary>
    public readonly struct SearchUndo
    {
        private static readonly Position[] EmptyWallCells = new Position[0];

        private readonly IReadOnlyList<Position> _wallCells;

        public SearchMoveKind Kind { get; }
        public PlayerId PlayerId { get; }
        public PlayerId PreviousCurrentPlayerId { get; }
        public int PreviousTurn { get; }
        public Position PreviousPawn { get; }
        public IReadOnlyList<Position> WallCells => _wallCells ?? EmptyWallCells;

        private SearchUndo(
            SearchMoveKind kind,
            PlayerId playerId,
            PlayerId previousCurrentPlayerId,
            int previousTurn,
            Position previousPawn,
            IReadOnlyList<Position> wallCells
        )
        {
            Kind = kind;
            PlayerId = Guard.ThrowIfNull(playerId, nameof(playerId));
            PreviousCurrentPlayerId = Guard.ThrowIfNull(
                previousCurrentPlayerId,
                nameof(previousCurrentPlayerId)
            );
            PreviousTurn = previousTurn;
            PreviousPawn = previousPawn;
            _wallCells = wallCells ?? EmptyWallCells;
        }

        public static SearchUndo ForMovePawn(
            PlayerId playerId,
            PlayerId previousCurrentPlayerId,
            int previousTurn,
            Position previousPawn
        )
        {
            return new SearchUndo(
                SearchMoveKind.MovePawn,
                playerId,
                previousCurrentPlayerId,
                previousTurn,
                previousPawn,
                EmptyWallCells
            );
        }

        public static SearchUndo ForPlaceWall(
            PlayerId playerId,
            PlayerId previousCurrentPlayerId,
            int previousTurn,
            IReadOnlyList<Position> wallCells
        )
        {
            return new SearchUndo(
                SearchMoveKind.PlaceWall,
                playerId,
                previousCurrentPlayerId,
                previousTurn,
                default,
                Guard.ThrowIfNull(wallCells, nameof(wallCells))
            );
        }
    }
}
