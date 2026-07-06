using NUnit.Framework;

namespace Quoridor.Tests
{
    [TestFixture]
    public sealed class SearchProfilerTests
    {
        [Test]
        public void End_ReturnsRecordedNodeAndBfsCounts()
        {
            var profiler = new SearchProfiler();

            profiler.Begin();
            profiler.RecordNode();
            profiler.RecordNode(2);
            profiler.RecordNodes(2);
            profiler.RecordNodes(3, 4);
            profiler.RecordBfsSearch();
            profiler.RecordBfsSearches(3);
            var snapshot = profiler.End();

            Assert.That(snapshot.NodeCount, Is.EqualTo(8));
            Assert.That(snapshot.NodeCountsByDepth, Is.EqualTo("2:1,3:4"));
            Assert.That(snapshot.BfsCount, Is.EqualTo(4));
            Assert.That(snapshot.GcAllocatedBytes, Is.GreaterThanOrEqualTo(0));
            Assert.That(snapshot.Elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(0));
            Assert.That(profiler.IsRunning, Is.False);
        }

        [Test]
        public void Reset_ClearsRecordedCounts()
        {
            var profiler = new SearchProfiler();

            profiler.Begin();
            profiler.RecordNode();
            profiler.RecordBfsSearch();
            profiler.Reset();
            var snapshot = profiler.CaptureSnapshot();

            Assert.That(snapshot.NodeCount, Is.EqualTo(0));
            Assert.That(snapshot.BfsCount, Is.EqualTo(0));
            Assert.That(profiler.IsRunning, Is.False);
        }

        [Test]
        public void Pathfinder_ThrowsArgumentNullException_WhenResolverIsNull()
        {
            var exception = Assert.Throws<System.ArgumentNullException>(
                () => new Pathfinder(null, new SearchProfiler())
            );

            Assert.That(exception.ParamName, Is.EqualTo("resolver"));
        }

        [Test]
        public void Pathfinder_ThrowsArgumentNullException_WhenProfilerIsNull()
        {
            var exception = Assert.Throws<System.ArgumentNullException>(
                () => new Pathfinder(new GoalResolver(), null)
            );

            Assert.That(exception.ParamName, Is.EqualTo("searchProfiler"));
        }

        [Test]
        public void Pathfinder_RecordsBfsSearches_WhenProfilerIsInjected()
        {
            var profiler = new SearchProfiler();
            var pathfinder = new Pathfinder(new GoalResolver(), profiler);
            var board = CreateOpenBoard();

            profiler.Begin();
            Assert.That(pathfinder.CanReachGoal(board, PlayerId.FirstPlayer), Is.True);
            Assert.That(pathfinder.GetShortestDistanceToGoal(board, PlayerId.SecondPlayer), Is.EqualTo(2));
            var snapshot = profiler.End();

            Assert.That(snapshot.BfsCount, Is.EqualTo(2));
        }

        [Test]
        public void SearchPathfinder_MatchesPathfinder_OnOpenBoard()
        {
            var pathfinder = new Pathfinder(new GoalResolver(), new SearchProfiler());
            var searchPathfinder = new SearchPathfinder(new GoalResolver(), new SearchProfiler());
            var board = CreateOpenBoard();

            Assert.That(searchPathfinder.CanReachGoal(board, PlayerId.FirstPlayer), Is.EqualTo(pathfinder.CanReachGoal(board, PlayerId.FirstPlayer)));
            Assert.That(searchPathfinder.CanReachGoal(board, PlayerId.SecondPlayer), Is.EqualTo(pathfinder.CanReachGoal(board, PlayerId.SecondPlayer)));
            Assert.That(searchPathfinder.GetShortestDistanceToGoal(board, PlayerId.FirstPlayer), Is.EqualTo(pathfinder.GetShortestDistanceToGoal(board, PlayerId.FirstPlayer)));
            Assert.That(searchPathfinder.GetShortestDistanceToGoal(board, PlayerId.SecondPlayer), Is.EqualTo(pathfinder.GetShortestDistanceToGoal(board, PlayerId.SecondPlayer)));
        }

        [Test]
        public void SearchPathfinder_MatchesPathfinder_WhenWallBlocksDirectRoute()
        {
            var pathfinder = new Pathfinder(new GoalResolver(), new SearchProfiler());
            var searchPathfinder = new SearchPathfinder(new GoalResolver(), new SearchProfiler());
            var board = CreateBoardWithHorizontalWall();

            Assert.That(searchPathfinder.CanReachGoal(board, PlayerId.FirstPlayer), Is.EqualTo(pathfinder.CanReachGoal(board, PlayerId.FirstPlayer)));
            Assert.That(searchPathfinder.CanReachGoal(board, PlayerId.SecondPlayer), Is.EqualTo(pathfinder.CanReachGoal(board, PlayerId.SecondPlayer)));
            Assert.That(searchPathfinder.GetShortestDistanceToGoal(board, PlayerId.FirstPlayer), Is.EqualTo(pathfinder.GetShortestDistanceToGoal(board, PlayerId.FirstPlayer)));
            Assert.That(searchPathfinder.GetShortestDistanceToGoal(board, PlayerId.SecondPlayer), Is.EqualTo(pathfinder.GetShortestDistanceToGoal(board, PlayerId.SecondPlayer)));
        }

        [Test]
        public void SearchPathfinder_ReusesWorkspaceAcrossDifferentBoardSizes()
        {
            var searchPathfinder = new SearchPathfinder(new GoalResolver(), new SearchProfiler());
            var smallBoard = CreateOpenBoard();
            var largeBoard = new BoardState(
                new int[7, 7],
                new[]
                {
                    new Position(2, 0),
                    new Position(4, 6),
                }
            );

            Assert.That(searchPathfinder.GetShortestDistanceToGoal(smallBoard, PlayerId.FirstPlayer), Is.EqualTo(2));
            Assert.That(searchPathfinder.GetShortestDistanceToGoal(largeBoard, PlayerId.SecondPlayer), Is.EqualTo(3));
            Assert.That(searchPathfinder.GetShortestDistanceToGoal(smallBoard, PlayerId.SecondPlayer), Is.EqualTo(2));
        }

        private static BoardState CreateBoardWithHorizontalWall()
        {
            var grid = new int[5, 5];
            grid[1, 1] = 1;
            grid[1, 2] = 1;
            grid[1, 3] = 1;

            return new BoardState(
                grid,
                new[]
                {
                    new Position(2, 0),
                    new Position(2, 4),
                }
            );
        }

        private static BoardState CreateOpenBoard()
        {
            return new BoardState(
                new int[5, 5],
                new[]
                {
                    new Position(2, 0),
                    new Position(2, 4),
                }
            );
        }
    }
}
