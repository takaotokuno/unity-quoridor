using System;
using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// αβ探索中に使う軽量な手表現。
    /// 最終的に選ばれた手だけを本番用コマンドへ変換する。
    /// </summary>
    public readonly struct SearchMove
    {
        private static readonly Position[] EmptyWallCells = new Position[0];

        private readonly Position[] _wallCells;

        public SearchMoveKind Kind { get; }
        public PlayerId PlayerId { get; }
        public Position Target { get; }
        public Position Origin { get; }
        public WallDirection Direction { get; }
        public IReadOnlyList<Position> WallCells => _wallCells ?? EmptyWallCells;

        private SearchMove(
            SearchMoveKind kind,
            PlayerId playerId,
            Position target,
            Position origin,
            WallDirection direction,
            Position[] wallCells
        )
        {
            Kind = kind;
            PlayerId = Guard.ThrowIfNull(playerId, nameof(playerId));
            Target = target;
            Origin = origin;
            Direction = direction;
            _wallCells = wallCells ?? EmptyWallCells;
        }

        public static SearchMove MovePawn(PlayerId playerId, Position target)
        {
            return new SearchMove(
                SearchMoveKind.MovePawn,
                playerId,
                target,
                default,
                default,
                EmptyWallCells
            );
        }

        public static SearchMove PlaceWall(
            PlayerId playerId,
            Position origin,
            WallDirection direction,
            IReadOnlyList<Position> wallCells
        )
        {
            Guard.ThrowIfNull(wallCells, nameof(wallCells));

            if (wallCells.Count == 0)
            {
                throw new ArgumentException("Wall cells must not be empty.", nameof(wallCells));
            }

            var cells = new Position[wallCells.Count];
            for (var i = 0; i < wallCells.Count; i++)
            {
                cells[i] = wallCells[i];
            }

            return new SearchMove(
                SearchMoveKind.PlaceWall,
                playerId,
                origin,
                default,
                direction,
                cells
            );
        }

        public UseSkillCommand ToUseSkillCommand(string issuer)
        {
            switch (Kind)
            {
                case SearchMoveKind.MovePawn:
                    return new UseSkillCommand(
                        PlayerId,
                        BuiltInSkillSlotIds.MovePawn,
                        Target,
                        issuer
                    );

                case SearchMoveKind.PlaceWall:
                    return new UseSkillCommand(
                        PlayerId,
                        BuiltInSkillSlotIds.PlaceWall,
                        Origin,
                        issuer
                    );

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, "Unknown search move kind.");
            }
        }
    }
}
