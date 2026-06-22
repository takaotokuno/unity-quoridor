namespace Quoridor
{
    /// <summary>
    /// CPU エージェントの行動選択を担当する Strategy。
    /// </summary>
    public interface ICpuAgentStrategy
    {
        /// <summary>
        /// 現在ターンの状態から発行するコマンドを決定する。
        /// 行動できない場合は null を返す。
        /// </summary>
        IMatchCommand DecideCommand(CpuAgentDecisionContext context);
    }
}
