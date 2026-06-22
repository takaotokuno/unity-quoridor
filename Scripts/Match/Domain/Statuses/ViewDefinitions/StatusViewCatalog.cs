using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Quoridor/Status View Catalog")]
    public sealed class StatusViewCatalog : ScriptableObject
    {
        [SerializeField] private List<StatusViewEntry> entries = new();

        public StatusViewEntry Find(StatusId statusId)
        {
            foreach (var entry in entries)
            {
                if (entry.StatusId == statusId)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}