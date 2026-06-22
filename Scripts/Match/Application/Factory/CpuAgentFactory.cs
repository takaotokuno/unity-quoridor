using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    public sealed class CpuAgentFactory
    {
        private readonly CpuAgentStrategyFactory _strategyFactory;

        public CpuAgentFactory(CpuAgentStrategyFactory strategyFactory)
        {
            _strategyFactory = Guard.ThrowIfNull(strategyFactory, nameof(strategyFactory));
        }

        /// <summary>
        /// <see cref="PlayerConfig.IsCpu"/> が true のプレイヤーに対して
        /// <see cref="MlAgentsCpuAgent"/> を生成・初期化して返す。
        /// CPU プレイヤーが存在しない場合は空のリストを返す。
        /// </summary>
        public IReadOnlyList<MlAgentsCpuAgent> Create(
            MatchStateConfig stateConfig,
            MatchState state,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus
        )
        {
            ValidateCreateArguments(
                stateConfig,
                state,
                commandPort,
                eventBus
            );

            var agents = new List<MlAgentsCpuAgent>();

            var playerConfigs = new[]
            {
                (playerId: PlayerId.FirstPlayer, config: stateConfig.PlayerFirst),
                (playerId: PlayerId.SecondPlayer, config: stateConfig.PlayerSecond),
            };

            foreach (var item in playerConfigs)
            {
                PlayerId playerId = item.playerId;
                PlayerConfig config = item.config;

                if (!config.IsCpu)
                    continue;

                var go = new GameObject($"CpuAgent_Player{playerId}");
                var agent = go.AddComponent<MlAgentsCpuAgent>();
                var strategy = _strategyFactory.Create(config.CpuStrategyKind);

                agent.Initialize(
                    playerId,
                    state,
                    commandPort,
                    eventBus,
                    strategy
                );

                agents.Add(agent);
            }

            return agents;
        }

        private static void ValidateCreateArguments(
            MatchStateConfig stateConfig,
            MatchState state,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus
        )
        {
            Guard.ThrowIfNull(stateConfig, nameof(stateConfig));
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(commandPort, nameof(commandPort));
            Guard.ThrowIfNull(eventBus, nameof(eventBus));
        }
    }
}
