using System;
using System.Collections;

namespace Quoridor
{
    /// <summary>
    /// depth 1 の探索として、合法手を 1 手先まで仮実行して最も評価値が高い手を選ぶ Strategy。
    /// αβ探索では depth 1 の前段に相当し、同じ列挙・仮実行・評価部品を再利用する。
    /// Match 固有の状態は持たず、DI で Singleton 登録して使い回せる。
    /// </summary>
    public sealed class GreedyCpuAgentStrategy : CpuSearchStrategyBase
    {
        public GreedyCpuAgentStrategy(
            LegalCommandEnumerator legalCommandEnumerator,
            CpuCommandSimulator commandSimulator,
            IRandomProvider randomProvider
        )
            : base(
                legalCommandEnumerator,
                commandSimulator,
                randomProvider
            )
        {
        }

        public override IEnumerator DecideCommand(
            CpuAgentDecisionContext context,
            Action<IMatchCommand> onDecided
        )
        {
            var candidates = EnumerateAllLegalCommands(context);
            if (candidates.Count == 0)
            {
                onDecided?.Invoke(null);
                yield break;
            }

            CpuBestCommandAccumulator bestCommands = CreateBestCommandAccumulator();

            foreach (IMatchCommand candidate in candidates)
            {
                CpuCommandSimulationResult simulation = SimulateCommand(context.State, candidate);
                if (!simulation.Succeeded)
                    continue;

                int score = Evaluate(context, simulation.State, context.PlayerId);
                bestCommands.Add(candidate, score);
            }

            CpuBestCommandSelection selection = bestCommands.Select();
            onDecided?.Invoke(selection.HasCommand ? selection.Command : null);
        }
    }
}
