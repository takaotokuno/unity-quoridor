using UnityEngine;
using System.Collections.Generic;
namespace Quoridor
{
    [CreateAssetMenu(menuName = "Effects/Background Effect Preset Catalog")]
    public sealed class BackgroundEffectCatalog : ScriptableObject
    {
        [SerializeField] private List<BackgroundEffectEntry> entries = new();

        public bool TryGet(BackgroundEffectPresetId presetId, out BackgroundEffectState state)
        {
            foreach (var entry in entries)
            {
                if (entry.PresetId == presetId)
                {
                    state = entry.State;
                    return true;
                }
            }

            state = default;
            return false;
        }
    }
}
