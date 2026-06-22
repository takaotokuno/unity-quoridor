namespace Quoridor
{
    /// <summary>
    /// CPU エージェントの共通インターフェース。
    /// ターン開始時に呼ばれ、エージェントが行動を決定する。
    /// </summary>
    public interface ICpuAgent
    {
        /// <summary>
        /// ターン開始時に呼ばれる。エージェントは <paramref name="state"/> を参照して
        /// <paramref name="playerId"/> のプレイヤーとして行動を決定・発行する。
        /// </summary>
        /// <param name="state">現在のゲーム状態</param>
        /// <param name="playerId">このエージェントが担当するプレイヤーID</param>
        void OnTurnStarted(MatchState state, PlayerId playerId);
    }
}
