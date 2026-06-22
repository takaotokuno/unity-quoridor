using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Quoridor/Status Catalog")]
    public sealed class StatusCatalog : ScriptableObject
    {
        [SerializeField] private List<StatusCatalogEntry> entries = new();

        public IReadOnlyList<StatusCatalogEntry> Entries => entries;
    }
}