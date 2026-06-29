namespace Quoridor
{
    /// <summary>
    /// CPU エージェントが使用する意思決定 Strategy の種類。
    /// </summary>
    public enum CpuAgentStrategyKind
    {
        /// <summary>
        /// すべての合法手からランダムに選択する汎用 Strategy。
        /// </summary>
        RandomLegal = 0,

        /// <summary>
        /// コマ移動だけを対象にランダムに選択する検証・デバッグ向け Strategy。
        /// </summary>
        MoveOnly = 1,

        /// <summary>
        /// 1 手先の局面評価が最も高い合法手を選択する Strategy。
        /// </summary>
        Greedy = 2,

        /// <summary>
        /// 一定時間の反復深化 αβ 探索で、時間到達時点の最善手を選択する Strategy。
        /// </summary>
        AlphaBeta = 3,
    }
}
