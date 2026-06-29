using System;
using System.Collections;

namespace Quoridor
{
    /// <summary>
    /// CPU Agent の意思決定に必要な Strategy と設定をまとめ、行動決定を実行する。
    /// </summary>
    public sealed class CpuAgentBehavior
    {
        private readonly ICpuAgentStrategy _strategy;
        private readonly CpuSearchTimeLimit _searchTimeLimit;
        private readonly ICpuEvaluator _evaluator;

        public CpuAgentBehavior(
            ICpuAgentStrategy strategy,
            CpuSearchTimeLimit searchTimeLimit,
            ICpuEvaluator evaluator
        )
        {
            _strategy = Guard.ThrowIfNull(strategy, nameof(strategy));
            _searchTimeLimit = searchTimeLimit ?? CpuSearchTimeLimit.Default;
            _evaluator = Guard.ThrowIfNull(evaluator, nameof(evaluator));
        }

        public IEnumerator DecideCommand(
            MatchState state,
            PlayerId playerId,
            string issuer,
            Action<IMatchCommand> onDecided
        )
        {
            var context = new CpuAgentDecisionContext(
                state,
                playerId,
                issuer,
                _searchTimeLimit,
                _evaluator
            );

            return _strategy.DecideCommand(context, onDecided);
        }
    }
}
