namespace Quoridor
{
    /// <summary>
    /// 探索 Strategy が局面を数値化する CPU 評価関数。
    /// </summary>
    public interface ICpuEvaluator
    {
        /// <summary>
        /// perspectivePlayerId から見た state の良さを返す。
        /// 値が大きいほど CPU にとって有利な局面を表す。
        /// </summary>
        int Evaluate(MatchState state, PlayerId perspectivePlayerId);
    }
}
