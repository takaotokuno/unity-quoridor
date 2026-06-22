using UnityEngine;

namespace Quoridor
{
    [System.Serializable]
    public sealed class StatusViewEntry
    {
        [SerializeField] private StatusId statusId;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        public StatusId StatusId => statusId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
    }
}