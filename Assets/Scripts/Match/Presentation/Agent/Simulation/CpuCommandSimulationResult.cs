namespace Quoridor
{
    /// <summary>
    /// 探索用にコマンドを仮実行した結果。
    /// </summary>
    public readonly struct CpuCommandSimulationResult
    {
        public bool Succeeded { get; }
        public MatchState State { get; }

        private CpuCommandSimulationResult(bool succeeded, MatchState state)
        {
            Succeeded = succeeded;
            State = state;
        }

        public static CpuCommandSimulationResult Success(MatchState state)
        {
            return new CpuCommandSimulationResult(true, state);
        }

        public static CpuCommandSimulationResult Failure()
        {
            return new CpuCommandSimulationResult(false, null);
        }
    }
}
