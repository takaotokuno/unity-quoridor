using System;

namespace Quoridor
{
    /// <summary>
    /// PlayerConfig に指定された用途に応じて CPU Strategy を構築する。
    /// </summary>
    public sealed class CpuAgentStrategyFactory
    {
        private readonly LegalCommandEnumerator _legalCommandEnumerator;
        private readonly IRandomProvider _randomProvider;

        public CpuAgentStrategyFactory(
            LegalCommandEnumerator legalCommandEnumerator,
            IRandomProvider randomProvider
        )
        {
            _legalCommandEnumerator = Guard.ThrowIfNull(legalCommandEnumerator, nameof(legalCommandEnumerator));
            _randomProvider = Guard.ThrowIfNull(randomProvider, nameof(randomProvider));
        }

        public ICpuAgentStrategy Create(CpuAgentStrategyKind kind)
        {
            switch (kind)
            {
                case CpuAgentStrategyKind.RandomLegal:
                    return new RandomLegalCpuAgentStrategy(
                        _legalCommandEnumerator,
                        _randomProvider,
                        LegalCommandEnumerationOptions.All
                    );

                case CpuAgentStrategyKind.MoveOnly:
                    return new RandomLegalCpuAgentStrategy(
                        _legalCommandEnumerator,
                        _randomProvider,
                        LegalCommandEnumerationOptions.MoveOnly
                    );

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown CPU agent strategy.");
            }
        }
    }
}
