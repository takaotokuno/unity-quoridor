using System;
using System.Collections;

namespace Quoridor
{
    /// <summary>
    /// コマ移動だけを対象に、合法手からランダムに 1 手を選ぶ Strategy。
    /// Match 固有の状態は持たず、DI で Singleton 登録して使い回せる。
    /// </summary>
    public sealed class MoveOnlyCpuAgentStrategy : ICpuAgentStrategy
    {
        private readonly LegalCommandEnumerator _legalCommandEnumerator;
        private readonly IRandomProvider _randomProvider;

        public MoveOnlyCpuAgentStrategy(
            LegalCommandEnumerator legalCommandEnumerator,
            IRandomProvider randomProvider
        )
        {
            _legalCommandEnumerator = Guard.ThrowIfNull(legalCommandEnumerator, nameof(legalCommandEnumerator));
            _randomProvider = Guard.ThrowIfNull(randomProvider, nameof(randomProvider));
        }

        public IEnumerator DecideCommand(
            CpuAgentDecisionContext context,
            Action<IMatchCommand> onDecided
        )
        {
            var candidates = _legalCommandEnumerator.Enumerate(context, LegalCommandEnumerationOptions.MoveOnly);
            if (candidates.Count == 0)
            {
                onDecided?.Invoke(null);
                yield break;
            }

            int index = _randomProvider.Range(0, candidates.Count);
            onDecided?.Invoke(candidates[index]);
        }
    }
}
