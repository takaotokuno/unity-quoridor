using UnityEngine;

namespace Quoridor
{
    [System.Serializable]
    public sealed class SkillParameterEntry
    {
        [SerializeField] private string key;
        [SerializeField] private int value;

        public string Key => key;
        public int Value => value;
    }
}