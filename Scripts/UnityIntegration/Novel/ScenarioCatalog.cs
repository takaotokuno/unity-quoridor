using UnityEngine;
using System.Collections.Generic;

namespace Quoridor
{
    [CreateAssetMenu(menuName = "Nobel/Scenario Catalog")]
    public sealed class ScenarioCatalog : ScriptableObject
    {
        [SerializeField] private List<ScenarioEntry> scenarioEntries = new();

        public string FindScenario(ScenarioId id)
        {
            foreach (var entry in scenarioEntries)
            {
                if (entry.Id == id) return entry.Label;
            }
            return null;
        }
    }
}
