using System;
using System.Collections;
using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// 評価関数ベースの CPU Strategy で共通利用する合法手列挙・仮実行・評価・最善手選択の基底処理。
    /// </summary>
    public abstract class CpuSearchStrategyBase : ICpuAgentStrategy
    {
        private readonly LegalCommandEnumerator _legalCommandEnumerator;
        private readonly CpuCommandSimulator _commandSimulator;
        private readonly IRandomProvider _randomProvider;

        protected CpuSearchStrategyBase(
            LegalCommandEnumerator legalCommandEnumerator,
            CpuCommandSimulator commandSimulator,
            IRandomProvider randomProvider
        )
        {
            _legalCommandEnumerator = Guard.ThrowIfNull(legalCommandEnumerator, nameof(legalCommandEnumerator));
            _commandSimulator = Guard.ThrowIfNull(commandSimulator, nameof(commandSimulator));
            _randomProvider = Guard.ThrowIfNull(randomProvider, nameof(randomProvider));
        }

        public abstract IEnumerator DecideCommand(
            CpuAgentDecisionContext context,
            Action<IMatchCommand> onDecided
        );

        protected List<IMatchCommand> EnumerateAllLegalCommands(CpuAgentDecisionContext context)
        {
            return _legalCommandEnumerator.Enumerate(context, LegalCommandEnumerationOptions.All);
        }

        protected CpuCommandSimulationResult SimulateCommand(MatchState state, IMatchCommand command)
        {
            return _commandSimulator.Simulate(state, command);
        }

        protected CpuCommandSimulationResult SimulateCompletedTurn(MatchState state, IMatchCommand command)
        {
            return _commandSimulator.SimulateTurn(state, command);
        }

        protected int Evaluate(
            CpuAgentDecisionContext context,
            MatchState state,
            PlayerId perspectivePlayerId
        )
        {
            ICpuEvaluator evaluator = Guard.ThrowIfNull(context.Evaluator, nameof(context.Evaluator));
            return evaluator.Evaluate(state, perspectivePlayerId);
        }

        protected IMatchCommand PickRandomCommand(IReadOnlyList<IMatchCommand> commands)
        {
            if (commands == null || commands.Count == 0)
                return null;

            int index = _randomProvider.Range(0, commands.Count);
            return commands[index];
        }

        protected CpuBestCommandAccumulator CreateBestCommandAccumulator()
        {
            return new CpuBestCommandAccumulator(_randomProvider);
        }
    }
}
