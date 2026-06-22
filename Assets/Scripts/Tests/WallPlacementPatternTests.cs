using System;
using NUnit.Framework;

namespace Quoridor.Tests
{
    [TestFixture]
    public sealed class WallPlacementPatternTests
    {
        [Test]
        public void Constructor_Throws_WhenCellsDoNotMatchLength()
        {
            Assert.Throws<ArgumentException>(() => new WallPlacementPattern(
                new Position(1, 0),
                WallDirection.Vertical,
                3,
                new[]
                {
                    new Position(1, 0),
                    new Position(1, 1),
                }
            ));
        }

        [Test]
        public void Constructor_Throws_WhenCellsAreNotContiguousFromOrigin()
        {
            Assert.Throws<ArgumentException>(() => new WallPlacementPattern(
                new Position(1, 0),
                WallDirection.Vertical,
                3,
                new[]
                {
                    new Position(1, 0),
                    new Position(1, 2),
                    new Position(1, 3),
                }
            ));
        }

        [Test]
        public void Constructor_CopiesCells_ToPreservePatternInvariant()
        {
            var cells = new[]
            {
                new Position(1, 0),
                new Position(1, 1),
                new Position(1, 2),
            };

            var pattern = new WallPlacementPattern(
                new Position(1, 0),
                WallDirection.Vertical,
                3,
                cells
            );

            cells[1] = new Position(5, 5);

            Assert.That(pattern.Cells[1], Is.EqualTo(new Position(1, 1)));
        }
    }
}
