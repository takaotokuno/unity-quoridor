using UnityEngine;

namespace Quoridor
{
    [CreateAssetMenu(
        fileName = "MatchViewPrefabCatalog",
        menuName = "Quoridor/Match View Prefab Catalog"
    )]
    public sealed class MatchViewPrefabCatalog : ScriptableObject
    {
        [field: SerializeField] public ResignButtonView ResignButtonPrefab { get; private set; }
        [field: SerializeField] public SkipButtonView SkipButtonPrefab { get; private set; }
        [field: SerializeField] public TileView TilePrefab { get; private set; }
        [field: SerializeField] public WallView WallPrefabVertical { get; private set; }
        [field: SerializeField] public WallView WallPrefabHorizontal { get; private set; }
        [field: SerializeField] public WallJointView WallPrefabJoint { get; private set; }
        [field: SerializeField] public PawnView PawnPrefabFirst { get; private set; }
        [field: SerializeField] public PawnView PawnPrefabSecond { get; private set; }
        [field: SerializeField] public TurnPanelView TurnPanelPrefab { get; private set; }
        [field: SerializeField] public SkillButtonView SkillButtonPrefab { get; private set; }
        [field: SerializeField] public StatusIconView StatusPrefab { get; private set; }
        [field: SerializeField] public StatusPanelView StatusPanelPrefab { get; private set; }
    }
}