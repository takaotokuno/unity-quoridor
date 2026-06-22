namespace Quoridor
{
    /// <summary>
    /// スキル定義パラメータのキー定数。
    /// SkillDefinition.GetInt() の呼び出し時に使用する。
    /// </summary>
    public static class SkillParameterKeys
    {
        /// <summary>コマの移動距離（マス数）</summary>
        public const string Distance = "Distance";

        /// <summary>壁の長さ（マス数）</summary>
        public const string Length = "Length";

        /// <summary>スキル使用後のクールダウンターン数</summary>
        public const string CoolDownTime = "CoolDownTime";

        /// <summary>ステータス付与系スキルの対象プレーヤー</summary>
        public const string TargetPlayer = "TargetPlayer";

        /// <summary>付与するステータスのID（ApplyStatus 系エフェクトで使用）</summary>
        public const string StatusId = "StatusId";
    }
}
