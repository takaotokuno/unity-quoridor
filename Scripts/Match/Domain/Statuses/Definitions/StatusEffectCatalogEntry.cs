using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [System.Serializable]
    public sealed class StatusEffectCatalogEntry
    {
        [SerializeField] private StatusEffectId effectId;
        [SerializeField] private List<StatusParameterEntry> parameters = new();

        public StatusEffectId EffectId => effectId;
        public IReadOnlyList<StatusParameterEntry> Parameters => parameters;
    }
}