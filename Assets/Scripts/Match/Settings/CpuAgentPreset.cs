using UnityEngine;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Quoridor/CPU Agent Preset")]
    public sealed class CpuAgentPreset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private CpuAgentStrategyKind strategyKind = CpuAgentStrategyKind.RandomLegal;
        [SerializeField] private int searchTimeLimitMilliseconds = CpuAgentOptions.DefaultSearchTimeLimitMilliseconds;
        [SerializeField] private CpuEvaluatorPreset evaluatorPreset;

        public string Id => id;
        public string DisplayName => displayName;
        public CpuAgentStrategyKind StrategyKind => strategyKind;
        public int SearchTimeLimitMilliseconds => searchTimeLimitMilliseconds;
        public CpuEvaluatorPreset EvaluatorPreset => evaluatorPreset;

        public CpuAgentOptions ToOptions()
        {
            return new CpuAgentOptions(
                strategyKind,
                searchTimeLimitMilliseconds,
                evaluatorPreset
            );
        }
    }
}
