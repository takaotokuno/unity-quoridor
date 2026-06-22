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
    }
}
