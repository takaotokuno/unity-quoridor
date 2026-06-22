using UnityEngine;

namespace Quoridor
{
    [System.Serializable]
    public sealed class StatusParameterEntry
    {
        [SerializeField] private string key;
        [SerializeField] private int value;

        public string Key => key;
        public int Value => value;
    }
}