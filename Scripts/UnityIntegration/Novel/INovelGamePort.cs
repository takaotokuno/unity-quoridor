using System;

namespace Quoridor
{
    public interface INovelGamePort
    {
        void JumpScenario(ScenarioId scenarioId);
        void JumpScenario(ScenarioId scenarioId, Action onComplete);
    }   
}
