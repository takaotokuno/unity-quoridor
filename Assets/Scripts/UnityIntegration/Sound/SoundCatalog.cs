using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Audio/Sound Catalog")]
    public sealed class SoundCatalog : ScriptableObject
    {
        [SerializeField] private List<BgmEntry> bgmEntries = new();
        [SerializeField] private List<SeEntry> seEntries = new();

        public AudioClip FindBgm(BgmId id)
        {
            foreach (var entry in bgmEntries)
            {
                if (entry.Id == id) return entry.Clip;
            }
            return null;
        }

        public AudioClip FindSe(SeId id)
        {
            foreach (var entry in seEntries)
            {
                if (entry.Id == id) return entry.Clip;
            }
            return null;
        }
    }
}
