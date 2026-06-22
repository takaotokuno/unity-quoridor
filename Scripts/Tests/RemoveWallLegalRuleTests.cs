using System.Collections.Generic;
using NUnit.Framework;

namespace Quoridor.Tests
{
    [TestFixture]
    public sealed class RemoveWallLegalRuleTests
    {
        [Test]
        public void IsLegal_ReturnsTrue_WhenTargetIsPatternOrigin()
        {
            var state = CreateStateWithWall(new[]
            {
                new Position(2, 1),
                new Position(3, 1),
                new Position(4, 1),
            });
            var definition = CreateRemoveWallDefinition();
            var rule = new RemoveWallLegalRule(CreateExistingWallPatternResolver());

            var result = rule.IsLegal(new SkillLegalContext(
                state,
                PlayerId.SecondPlayer,
                definition,
                new Position(2, 1)
            ));

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsLegal_ReturnsFalse_WhenTargetIsMiddleSegmentAndNotPatternOrigin()
        {
            var state = CreateStateWithWall(new[]
            {
                new Position(2, 1),
                new Position(3, 1),
                new Position(4, 1),
            });
            var definition = CreateRemoveWallDefinition();
            var rule = new RemoveWallLegalRule(CreateExistingWallPatternResolver());

            var result = rule.IsLegal(new SkillLegalContext(
                state,
                PlayerId.SecondPlayer,
                definition,
                new Position(3, 1)
            ));

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsLegal_ReturnsFalse_WhenTargetIsEndSegmentAndNotPatternOrigin()
        {
            var state = CreateStateWithWall(new[]
            {
                new Position(2, 1),
                new Position(3, 1),
                new Position(4, 1),
            });
            var definition = CreateRemoveWallDefinition();
            var rule = new RemoveWallLegalRule(CreateExistingWallPatternResolver());

            var result = rule.IsLegal(new SkillLegalContext(
                state,
                PlayerId.SecondPlayer,
                definition,
                new Position(4, 1)
            ));

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsLegal_ReturnsFalse_WhenTargetWallHasNoCompletePattern()
        {
            var state = CreateStateWithWall(new[]
            {
                new Position(4, 1),
            });
            var definition = CreateRemoveWallDefinition();
            var rule = new RemoveWallLegalRule(CreateExistingWallPatternResolver());

            var result = rule.IsLegal(new SkillLegalContext(
                state,
                PlayerId.SecondPlayer,
                definition,
                new Position(4, 1)
            ));

            Assert.That(result, Is.False);
        }

        [Test]
        public void Compose_RemovesExistingPattern_WhenTargetIsPatternOrigin()
        {
            var state = CreateStateWithWall(new[]
            {
                new Position(2, 1),
                new Position(3, 1),
                new Position(4, 1),
            });
            var definition = CreateRemoveWallDefinition();
            var composer = new RemoveWallEffectComposer(CreateExistingWallPatternResolver());

            composer.Compose(new SkillEffectContext(
                state,
                PlayerId.SecondPlayer,
                definition,
                new Position(2, 1)
            ));

            Assert.That(BoardGeometry.IsEmpty(state.Board, new Position(2, 1)), Is.True);
            Assert.That(BoardGeometry.IsEmpty(state.Board, new Position(3, 1)), Is.True);
            Assert.That(BoardGeometry.IsEmpty(state.Board, new Position(4, 1)), Is.True);
        }

        private static ExistingWallPatternResolver CreateExistingWallPatternResolver()
        {
            return new ExistingWallPatternResolver(new WallPlacementPatternProvider());
        }

        private static MatchState CreateStateWithWall(IReadOnlyList<Position> walls)
        {
            var grid = new int[17, 17];
            foreach (var wall in walls)
            {
                grid[wall.Y, wall.X] = 1;
            }

            var board = new BoardState(
                grid,
                new[]
                {
                    new Position(8, 0),
                    new Position(8, 16),
                }
            );

            return new MatchState(
                board,
                new[]
                {
                    CreatePlayer(PlayerId.FirstPlayer),
                    CreatePlayer(PlayerId.SecondPlayer),
                },
                new TurnState(PlayerSide.Second),
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
                new PlayerRuntimeState(isCpu: false)
            );
        }

        private static SkillDefinition CreateRemoveWallDefinition()
        {
            return new SkillDefinition(
                SkillId.Of("remove_wall"),
                SkillActivationType.BoardTarget,
                SkillTargetKind.Wall,
                true,
                3,
                SkillEffectComposerId.Of("remove_wall"),
                SkillLegalRuleId.Of("remove_wall"),
                new Dictionary<string, int>
                {
                    [SkillParameterKeys.Length] = 3,
                }
            );
        }
    }
}
