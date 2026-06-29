using System;
using System.Diagnostics;

namespace Quoridor
{
    /// <summary>
    /// αβ探索の性能を比較するための軽量プロファイラ。
    /// ノード数、BFS 実行回数、現在スレッドの GC Alloc 差分、経過時間を記録する。
    /// </summary>
    public sealed class SearchProfiler
    {
        private static readonly Func<long> AllocatedBytesProvider = CreateAllocatedBytesProvider();

        private readonly Stopwatch _stopwatch = new();
        private long _startAllocatedBytes;

        public long NodeCount { get; private set; }
        public long BfsCount { get; private set; }
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 計測値をリセットして、新しい探索計測を開始する。
        /// </summary>
        public void Begin()
        {
            ResetCounters();
            _startAllocatedBytes = GetAllocatedBytesForCurrentThread();
            _stopwatch.Restart();
            IsRunning = true;
        }

        /// <summary>
        /// 現在の探索計測を終了し、最終スナップショットを返す。
        /// </summary>
        public SearchProfilerSnapshot End()
        {
            if (IsRunning)
            {
                _stopwatch.Stop();
                IsRunning = false;
            }

            return CreateSnapshot();
        }

        /// <summary>
        /// 計測値をすべて初期状態に戻す。
        /// </summary>
        public void Reset()
        {
            _stopwatch.Reset();
            IsRunning = false;
            ResetCounters();
            _startAllocatedBytes = GetAllocatedBytesForCurrentThread();
        }

        /// <summary>
        /// 探索ノードを 1 つ訪問したことを記録する。
        /// </summary>
        public void RecordNode()
        {
            RecordNodes(1);
        }

        /// <summary>
        /// 探索ノードを複数まとめて訪問したことを記録する。
        /// </summary>
        public void RecordNodes(long count)
        {
            if (count <= 0)
            {
                return;
            }

            NodeCount += count;
        }

        /// <summary>
        /// BFS を 1 回実行したことを記録する。
        /// </summary>
        public void RecordBfsSearch()
        {
            RecordBfsSearches(1);
        }

        /// <summary>
        /// BFS 実行回数を複数まとめて記録する。
        /// </summary>
        public void RecordBfsSearches(long count)
        {
            if (count <= 0)
            {
                return;
            }

            BfsCount += count;
        }

        /// <summary>
        /// 計測中でも途中経過を取得できるスナップショットを返す。
        /// </summary>
        public SearchProfilerSnapshot CaptureSnapshot()
        {
            return CreateSnapshot();
        }

        private void ResetCounters()
        {
            NodeCount = 0;
            BfsCount = 0;
        }

        private SearchProfilerSnapshot CreateSnapshot()
        {
            var allocatedBytes = GetAllocatedBytesForCurrentThread() - _startAllocatedBytes;
            if (allocatedBytes < 0)
            {
                allocatedBytes = 0;
            }

            return new SearchProfilerSnapshot(
                NodeCount,
                BfsCount,
                allocatedBytes,
                _stopwatch.Elapsed
            );
        }

        private static long GetAllocatedBytesForCurrentThread()
        {
            return AllocatedBytesProvider();
        }

        private static Func<long> CreateAllocatedBytesProvider()
        {
            var method = typeof(GC).GetMethod(
                "GetAllocatedBytesForCurrentThread",
                Type.EmptyTypes
            );

            if (method == null)
            {
                return () => GC.GetTotalMemory(false);
            }

            return (Func<long>)Delegate.CreateDelegate(typeof(Func<long>), method);
        }
    }

    /// <summary>
    /// SearchProfiler の計測結果。
    /// </summary>
    public readonly struct SearchProfilerSnapshot
    {
        public long NodeCount { get; }
        public long BfsCount { get; }
        public long GcAllocatedBytes { get; }
        public TimeSpan Elapsed { get; }

        public SearchProfilerSnapshot(
            long nodeCount,
            long bfsCount,
            long gcAllocatedBytes,
            TimeSpan elapsed
        )
        {
            NodeCount = nodeCount;
            BfsCount = bfsCount;
            GcAllocatedBytes = gcAllocatedBytes;
            Elapsed = elapsed;
        }

        public override string ToString()
        {
            return $"nodes={NodeCount}, bfs={BfsCount}, gcAlloc={GcAllocatedBytes}B, elapsed={Elapsed.TotalMilliseconds:0.###}ms";
        }
    }
}
