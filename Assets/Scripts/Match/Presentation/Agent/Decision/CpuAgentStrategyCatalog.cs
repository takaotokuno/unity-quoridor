using System;
using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// DI に登録済みの CPU Strategy を PlayerConfig の指定に応じて返す。
    /// Strategy は Match 固有の状態を持たないため、ここでは生成せず参照だけを解決する。
    /// </summary>
    public sealed class CpuAgentStrategyCatalog
    {
        private readonly IReadOnlyDictionary<CpuAgentStrategyKind, ICpuAgentStrategy> _strategies;

        public CpuAgentStrategyCatalog(
            RandomLegalCpuAgentStrategy randomLegalStrategy,
            MoveOnlyCpuAgentStrategy moveOnlyStrategy,
            GreedyCpuAgentStrategy greedyStrategy,
            AlphaBetaCpuAgentStrategy alphaBetaStrategy
        )
        {
            _strategies = new Dictionary<CpuAgentStrategyKind, ICpuAgentStrategy>
            {
                {
                    CpuAgentStrategyKind.RandomLegal,
                    Guard.ThrowIfNull(randomLegalStrategy, nameof(randomLegalStrategy))
                },
                {
                    CpuAgentStrategyKind.MoveOnly,
                    Guard.ThrowIfNull(moveOnlyStrategy, nameof(moveOnlyStrategy))
                },
                {
                    CpuAgentStrategyKind.Greedy,
                    Guard.ThrowIfNull(greedyStrategy, nameof(greedyStrategy))
                },
                {
                    CpuAgentStrategyKind.AlphaBeta,
                    Guard.ThrowIfNull(alphaBetaStrategy, nameof(alphaBetaStrategy))
                },
            };
        }

        public ICpuAgentStrategy Get(CpuAgentStrategyKind kind)
        {
            if (_strategies.TryGetValue(kind, out ICpuAgentStrategy strategy))
                return strategy;

            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown CPU agent strategy.");
        }
    }
}
