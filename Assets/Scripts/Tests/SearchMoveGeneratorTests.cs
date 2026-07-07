using System.Collections.Generic;
using NUnit.Framework;

namespace Quoridor.Tests
{
    [TestFixture]
    public sealed class SearchMoveGeneratorTests
    {
        [Test]
        public void Generate_AddsPawnMovesMatchingMovePawnValidator()
        {
            var matchState = CreateMatchState(new int[5, 5]);
            var generator = CreateGenerator();
            var moves = new List<SearchMove>();

            generator.Generate(new SearchState(matchState), moves);

            var validator = new MovePawnValidator();
            var expected = validator.EnumerateLegalPositions(matchState, PlayerId.FirstPlayer, 1);
            Assert.That(ExtractPawnTargets(moves), Is.EquivalentTo(expected));
        }

        [Test]
        public void Generate_AddsWallMovesMatchingPlaceWallValidatorAndPatternProvider()
        {
            var matchState = CreateMatchState(new int[5, 5]);
            var generator = CreateGenerator();
            var moves = new List<SearchMove>();

            generator.Generate(new SearchState(matchState), moves);

            var patternProvider = new WallPlacementPatternProvider();
            var validator = new PlaceWallValidator(new Pathfinder(new GoalResolver(), new SearchProfiler()));
            var expectedOrigins = new List<Position>();
            foreach (var pattern in patternProvider.GetPatterns(matchState.Board, 3))
            {
                if (validator.CanPlaceWall(matchState, pattern))
                {
                    expectedOrigins.Add(pattern.Origin);
                }
            }

            Assert.That(ExtractWallOrigins(moves), Is.EquivalentTo(expectedOrigins));
        }

        [Test]
        public void Generate_ClearsAndReusesCallerBuffer()
        {
            var matchState = CreateMatchState(new int[5, 5]);
            var generator = CreateGenerator();
            var moves = new List<SearchMove>
            {
                SearchMove.MovePawn(PlayerId.SecondPlayer, new Position(0, 0))
            };

            generator.Generate(new SearchState(matchState), moves);

            Assert.That(moves, Has.All.Matches<SearchMove>(move => move.PlayerId == PlayerId.FirstPlayer));
        }

        private static SearchMoveGenerator CreateGenerator()
        {
            return new SearchMoveGenerator(new SearchPathfinder(new GoalResolver(), new SearchProfiler()));
        }

        private static IReadOnlyList<Position> ExtractPawnTargets(IEnumerable<SearchMove> moves)
        {
            var targets = new List<Position>();
            foreach (var move in moves)
            {
                if (move.Kind == SearchMoveKind.MovePawn)
                {
                    targets.Add(move.Target);
                }
            }

            return targets;
        }

        private static IReadOnlyList<Position> ExtractWallOrigins(IEnumerable<SearchMove> moves)
        {
            var origins = new List<Position>();
            foreach (var move in moves)
            {
                if (move.Kind == SearchMoveKind.PlaceWall)
                {
                    origins.Add(move.Origin);
                }
            }

            return origins;
        }

        private static MatchState CreateMatchState(int[,] grid)
        {
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
                new Dictionary<SkillSlotId, SkillState>
                {
                    { BuiltInSkillSlotIds.MovePawn, new SkillState(SkillId.NormalMovePawn, null, 0, 0, null) },
                    { BuiltInSkillSlotIds.PlaceWall, new SkillState(SkillId.NormalPlaceWall, null, 0, 0, null) },
                },
                new List<StatusState>(),
                new PlayerRuntimeState()
            );
        }
    }
}
