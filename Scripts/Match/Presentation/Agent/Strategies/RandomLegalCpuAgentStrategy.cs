namespace Quoridor
{
    /// <summary>
    /// 列挙された合法手からランダムに 1 手を選ぶ Strategy。
    /// </summary>
    public sealed class RandomLegalCpuAgentStrategy : ICpuAgentStrategy
    {
        private readonly LegalCommandEnumerator _legalCommandEnumerator;
        private readonly IRandomProvider _randomProvider;
        private readonly LegalCommandEnumerationOptions _options;

        public RandomLegalCpuAgentStrategy(
            LegalCommandEnumerator legalCommandEnumerator,
            IRandomProvider randomProvider,
            LegalCommandEnumerationOptions options
        )
        {
            _legalCommandEnumerator = Guard.ThrowIfNull(legalCommandEnumerator, nameof(legalCommandEnumerator));
            _randomProvider = Guard.ThrowIfNull(randomProvider, nameof(randomProvider));
            _options = options;
        }

        public IMatchCommand DecideCommand(CpuAgentDecisionContext context)
        {
            var candidates = _legalCommandEnumerator.Enumerate(context, _options);
            if (candidates.Count == 0)
                return null;

            int index = _randomProvider.Range(0, candidates.Count);
            return candidates[index];
        }
    }
}
