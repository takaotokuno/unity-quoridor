using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    public sealed class CpuAgentFactory
    {
        private readonly CpuAgentStrategyCatalog _strategyCatalog;
        private readonly CpuEvaluatorFactory _evaluatorFactory;
        public CpuAgentFactory(
            CpuAgentStrategyCatalog strategyCatalog,
            CpuEvaluatorFactory evaluatorFactory
        )
        {
            _strategyCatalog = Guard.ThrowIfNull(strategyCatalog, nameof(strategyCatalog));
            _evaluatorFactory = Guard.ThrowIfNull(evaluatorFactory, nameof(evaluatorFactory));
        }

        /// <summary>
        /// <see cref="PlayerConfig.IsCpu"/> が true のプレイヤーに対して
        /// <see cref="CpuAgent"/> を生成・初期化して返す。
        /// CPU プレイヤーが存在しない場合は空のリストを返す。
        /// </summary>
        public IReadOnlyList<CpuAgent> Create(
            MatchStateConfig stateConfig,
            MatchState state,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            Transform parent
        )
        {
            ValidateCreateArguments(
                stateConfig,
                state,
                commandPort,
                eventBus,
                parent
            );

            var agents = new List<CpuAgent>();
            Transform agentsRoot = null;

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

                if (agentsRoot == null)
                    agentsRoot = CreateAgentsRoot(parent);

                var go = new GameObject($"CpuAgent_Player{playerId}");
                go.transform.SetParent(agentsRoot, false);
              
                var agent = go.AddComponent<CpuAgent>();
                CpuAgentBehavior behavior = CreateBehavior(config.CpuOptions);

                agent.Initialize(
                    playerId,
                    state,
                    commandPort,
                    eventBus,
                    behavior
                );

                agents.Add(agent);
            }

            return agents;
        }

        private CpuAgentBehavior CreateBehavior(CpuAgentOptions options)
        {
            options = options ?? CpuAgentOptions.Default;
            return new CpuAgentBehavior(
                _strategyCatalog.Get(options.StrategyKind),
                options.SearchTimeLimit,
                CreateEvaluator(options)
            );
        }

        private ICpuEvaluator CreateEvaluator(CpuAgentOptions options)
        {
            if (options.EvaluatorPreset == null)
                return _evaluatorFactory.CreateDefault(CpuEvaluatorProcessorId.ShortestPath);

            return _evaluatorFactory.Create(options.EvaluatorPreset);
        }

        private static Transform CreateAgentsRoot(Transform parent)
        {
            var root = new GameObject("CpuAgents");
            root.transform.SetParent(parent, false);
            return root.transform;
        }

        private static void ValidateCreateArguments(
            MatchStateConfig stateConfig,
            MatchState state,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            Transform parent
        )
        {
            Guard.ThrowIfNull(stateConfig, nameof(stateConfig));
            Guard.ThrowIfNull(state, nameof(state));
            Guard.ThrowIfNull(commandPort, nameof(commandPort));
            Guard.ThrowIfNull(eventBus, nameof(eventBus));
            Guard.ThrowIfNull(parent, nameof(parent));
        }
    }
}
