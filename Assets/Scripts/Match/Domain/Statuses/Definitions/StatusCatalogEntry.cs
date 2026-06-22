using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [System.Serializable]
    public sealed class StatusCatalogEntry
    {
        [SerializeField] private StatusId statusId;
        [SerializeField] private List<StatusEffectCatalogEntry> effectEntries;
        [SerializeField] private StatusReapplyPolicy reapplyPolicy;

        public StatusId StatusId => statusId;
        public IReadOnlyList<StatusEffectCatalogEntry> EffectEntries => effectEntries;
        public StatusReapplyPolicy ReapplyPolicy => reapplyPolicy;
    }
}