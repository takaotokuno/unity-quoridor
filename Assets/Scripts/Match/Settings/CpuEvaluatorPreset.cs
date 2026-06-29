using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    /// <summary>
    /// CPU Evaluator の評価ロジック種別と調整パラメータを保持する Asset。
    /// </summary>
    [CreateAssetMenu(
        fileName = "CpuEvaluatorPreset",
        menuName = "Quoridor/CPU Evaluator Preset"
    )]
    public sealed class CpuEvaluatorPreset : ScriptableObject
    {
        [SerializeField] private string evaluatorId;
        [SerializeField] private string processorId = "shortest_path";
        [SerializeField] private List<CpuEvaluatorParameterEntry> parameters = new();

        public CpuEvaluatorId EvaluatorId => CpuEvaluatorId.Of(evaluatorId);
        public CpuEvaluatorProcessorId ProcessorId => CpuEvaluatorProcessorId.Of(processorId);
        public IReadOnlyList<CpuEvaluatorParameterEntry> Parameters => parameters;
    }
}
