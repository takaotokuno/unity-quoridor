using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    public enum PlayerControlMode
    {
        Manual = 0,
        Cpu = 1
    }

    [CreateAssetMenu(
        fileName = "EnemyMatchPreset",
        menuName = "Quoridor/Enemy Match Preset"
    )]
    public sealed class EnemyMatchPreset : ScriptableObject
    {
        [field: SerializeField] public int BoardSize { get; private set; } = 17;
        [field: SerializeField] public PlayerSide StartingSide { get; private set; }
        [field: SerializeField] public SerializablePosition[] InitPawns { get; private set; }

        [field: SerializeField] public PlayerControlMode PlayerControlMode { get; private set; } = PlayerControlMode.Manual;
        [field: SerializeField] public CpuAgentPreset PlayerCpuAgentPreset { get; private set; }
        [field: SerializeField] public List<string> PlayerSkillIds { get; private set; }

        [field: SerializeField] public PlayerControlMode EnemyControlMode { get; private set; } = PlayerControlMode.Manual;
        [field: SerializeField] public CpuAgentPreset EnemyCpuAgentPreset { get; private set; }
        [field: SerializeField] public List<string> EnemySkillIds { get; private set; }

        [field: SerializeField] public MatchViewPrefabCatalog ViewPrefabCatalog { get; private set; }
    }
}
