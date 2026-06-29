using UnityEngine;

namespace Quoridor
{
    /// <summary>
    /// CPU Evaluator Preset が保持する整数パラメータの 1 要素。
    /// </summary>
    [System.Serializable]
    public sealed class CpuEvaluatorParameterEntry
    {
        [SerializeField] private string key;
        [SerializeField] private int value;

        public string Key => key;
        public int Value => value;
    }
}
