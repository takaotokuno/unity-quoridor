using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchStateFactory
    {
        private readonly ISkillDefinitionRegistry _skillDefinitionRegistry;
        public MatchStateFactory(
            ISkillDefinitionRegistry skillDefinitionRegistry
        )
        {
            _skillDefinitionRegistry = skillDefinitionRegistry;   
        }

        public MatchState Create(MatchStateConfig config)
        {
            var grid = new int[config.BoardSize, config.BoardSize];
            var board = new BoardState(
                grid, 
                config.InitPawns
            );

            var players = new PlayerState[2];

            players[0] = new PlayerState(
                PlayerId.FirstPlayer,
                true, // isActive
                CreateSkillStates(config.PlayerFirst.SkillIds),
                new List<StatusState>(),
                new PlayerRuntimeState(isCpu: config.PlayerFirst.IsCpu)
            );

            players[1] = new PlayerState(
                PlayerId.SecondPlayer,
                true,
                CreateSkillStates(config.PlayerSecond.SkillIds),
                new List<StatusState>(),
                new PlayerRuntimeState(isCpu: config.PlayerSecond.IsCpu)
            );

            var turn = new TurnState(config.StartingSide);
            
            return new MatchState(
                board, 
                players, 
                turn
            );
        }

        private Dictionary<SkillSlotId, SkillState> CreateSkillStates(IReadOnlyList<SkillId> skillIds)
        {
            var skills = new Dictionary<SkillSlotId, SkillState>();

            for (int i = 0; i<skillIds.Count; i++)
            {
                var definition = _skillDefinitionRegistry.Find(skillIds[i]);
                skills.Add(
                    new SkillSlotId(i+1)
                    , new SkillState
                    (
                        definition.SkillId,
                        definition.MaxUseCount,
                        definition.GetInt(SkillParameterKeys.CoolDownTime),
                        0,
                        0
                    )
                );   
            }

            return skills;
        }
    }
}