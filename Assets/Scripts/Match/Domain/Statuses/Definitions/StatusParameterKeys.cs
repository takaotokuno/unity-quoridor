namespace Quoridor
{
    /// <summary>
    /// ステータス定義パラメータのキー定数。
    /// StatusDefinition.GetInt() の呼び出し時に使用する。
    /// </summary>
    public static class StatusParameterKeys
    {
        /// <summary>ステータスの残りターン数</summary>
        public const string RemainingTurns = "RemainingTurns";

        /// <summary>ステータス効果が発動するまでのクールダウン残数</summary>
        public const string CoolDownRemaining = "CoolDownRemaining";

        /// <summary>効果量（回復・ダメージ等の数値）</summary>
        public const string Amount = "Amount";

        /// <summary>発動確率があるステータス</summary>
        public const string Rate = "Rate";
    }
}
