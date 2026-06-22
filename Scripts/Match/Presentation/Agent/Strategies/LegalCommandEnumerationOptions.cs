namespace Quoridor
{
    /// <summary>
    /// 合法手列挙で対象にする行動カテゴリ。
    /// </summary>
    public readonly struct LegalCommandEnumerationOptions
    {
        public bool IncludeMovePawn { get; }
        public bool IncludePlaceWall { get; }
        public bool IncludeSpecialSkills { get; }

        public LegalCommandEnumerationOptions(
            bool includeMovePawn,
            bool includePlaceWall,
            bool includeSpecialSkills
        )
        {
            IncludeMovePawn = includeMovePawn;
            IncludePlaceWall = includePlaceWall;
            IncludeSpecialSkills = includeSpecialSkills;
        }

        public static LegalCommandEnumerationOptions All => new(
            includeMovePawn: true,
            includePlaceWall: true,
            includeSpecialSkills: true
        );

        public static LegalCommandEnumerationOptions MoveOnly => new(
            includeMovePawn: true,
            includePlaceWall: false,
            includeSpecialSkills: false
        );
    }
}
