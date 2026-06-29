using System.Collections.Generic;
using NUnit.Framework;

namespace Quoridor.Tests
{
    [TestFixture]
    public sealed class SearchStateTests
    {
        [Test]
        public void Constructor_CopiesBoardPawnsAndTurnFromMatchState()
        {
            var state = CreateMatchStateWithWall(new Position(1, 0));

            var searchState = new SearchState(state);

            Assert.That(searchState.Width, Is.EqualTo(5));
            Assert.That(searchState.Height, Is.EqualTo(5));
            Assert.That(searchState.CurrentPlayerId, Is.EqualTo(PlayerId.FirstPlayer));
            Assert.That(searchState.CurrentTurn, Is.EqualTo(1));
            Assert.That(searchState.GetPawn(PlayerId.FirstPlayer), Is.EqualTo(new Position(2, 0)));
            Assert.That(searchState.GetPawn(PlayerId.SecondPlayer), Is.EqualTo(new Position(2, 4)));
            Assert.That(searchState.IsWall(new Position(1, 0)), Is.True);
        }

        [Test]
        public void ApplyAndUndo_MovePawn_UpdatesAndRestoresPawnAndTurn()
        {
            var searchState = new SearchState(CreateMatchState());
            var move = SearchMove.MovePawn(PlayerId.FirstPlayer, new Position(2, 2));

            var undo = searchState.Apply(move);

            Assert.That(searchState.GetPawn(PlayerId.FirstPlayer), Is.EqualTo(new Position(2, 2)));
            Assert.That(searchState.CurrentPlayerId, Is.EqualTo(PlayerId.SecondPlayer));
            Assert.That(searchState.CurrentTurn, Is.EqualTo(2));

            searchState.Undo(move, undo);

            Assert.That(searchState.GetPawn(PlayerId.FirstPlayer), Is.EqualTo(new Position(2, 0)));
            Assert.That(searchState.CurrentPlayerId, Is.EqualTo(PlayerId.FirstPlayer));
            Assert.That(searchState.CurrentTurn, Is.EqualTo(1));
        }

        [Test]
        public void ApplyAndUndo_PlaceWall_UpdatesAndRestoresWallsAndTurn()
        {
            var searchState = new SearchState(CreateMatchState());
            var walls = new[]
            {
                new Position(1, 1),
                new Position(2, 1),
                new Position(3, 1),
            };
            var move = SearchMove.PlaceWall(
                PlayerId.FirstPlayer,
                walls[0],
                WallDirection.Horizontal,
                walls
            );

            var undo = searchState.Apply(move);

            foreach (var wall in walls)
            {
                Assert.That(searchState.IsWall(wall), Is.True);
            }
            Assert.That(searchState.CurrentPlayerId, Is.EqualTo(PlayerId.SecondPlayer));
            Assert.That(searchState.CurrentTurn, Is.EqualTo(2));

            searchState.Undo(move, undo);

            foreach (var wall in walls)
            {
                Assert.That(searchState.IsWall(wall), Is.False);
            }
            Assert.That(searchState.CurrentPlayerId, Is.EqualTo(PlayerId.FirstPlayer));
            Assert.That(searchState.CurrentTurn, Is.EqualTo(1));
        }

        [Test]
        public void Apply_ThrowsInvalidOperationException_WhenMovePlayerIsNotCurrentPlayer()
        {
            var searchState = new SearchState(CreateMatchState());
            var move = SearchMove.MovePawn(PlayerId.SecondPlayer, new Position(2, 2));

            Assert.Throws<System.InvalidOperationException>(() => searchState.Apply(move));
        }

        [Test]
        public void ToUseSkillCommand_ConvertsSearchMoveToBuiltInSkillCommand()
        {
            var move = SearchMove.MovePawn(PlayerId.FirstPlayer, new Position(2, 2));

            var command = move.ToUseSkillCommand(MatchCommandIssuers.CpuAgent);

            Assert.That(command.PlayerId, Is.EqualTo(PlayerId.FirstPlayer));
            Assert.That(command.SkillSlotId, Is.EqualTo(BuiltInSkillSlotIds.MovePawn));
            Assert.That(command.Target, Is.EqualTo(new Position(2, 2)));
            Assert.That(command.Issuer, Is.EqualTo(MatchCommandIssuers.CpuAgent));
        }

        private static MatchState CreateMatchState()
        {
            return CreateMatchStateWithWall(null);
        }

        private static MatchState CreateMatchStateWithWall(Position? wall)
        {
            var grid = new int[5, 5];
            if (wall.HasValue)
            {
                grid[wall.Value.Y, wall.Value.X] = 1;
            }

            var board = new BoardState(
                grid,
                new[]
                {
                    new Position(2, 0),
                    new Position(2, 4),
                }
            );

            return new MatchState(
                board,
                new[]
                {
                    CreatePlayer(PlayerId.FirstPlayer),
                    CreatePlayer(PlayerId.SecondPlayer),
                },
                new TurnState(PlayerSide.First),
                MatchPhase.InProgress
            );
        }

        private static PlayerState CreatePlayer(PlayerId playerId)
        {
            return new PlayerState(
                playerId,
                true,
                new Dictionary<SkillSlotId, SkillState>(),
                new List<StatusState>(),
                new PlayerRuntimeState()
            );
        }
    }
}
