using UnityEngine;

namespace Quoridor
{
    public sealed class ObjectLayoutView : MonoBehaviour
    {
        [field: SerializeField] public CanvasView CanvasView { get; private set; }
        [field: SerializeField] public Transform TurnPanel { get; private set; }
        [field: SerializeField] public MatchControlView MatchControlView { get; private set; }
        [field: SerializeField] public Transform ResignButton { get; private set; }
        [field: SerializeField] public Transform SkipButton { get; private set; }
        [field: SerializeField] public SkillButtonSetView SkillButtonSetViewFirst { get; private set; }
        [field: SerializeField] public SkillButtonSetView SkillButtonSetViewSecond { get; private set; }
        [field: SerializeField] public Transform StatusPanelFirst { get; private set; }
        [field: SerializeField] public Transform StatusPanelSecond { get; private set; }
        [field: SerializeField] public BoardView BoardView { get; private set; }

        [Header("Layout")]
        [SerializeField] private float cellSpacing = 1.0f;

        [Header("Depth Offset")]
        [SerializeField] private float pawnZOffset = 0.0f;
        [SerializeField] private float wallZOffset = 0.0f;

        private void Awake()
        {
            CanvasView.Hide();
        }

        public Vector3 GetCellPosition(int x, int y)
        {
            Vector3 origin = BoardView != null
                ? BoardView.transform.position
                : transform.position;

            return origin + new Vector3(
                x * cellSpacing,
                y * cellSpacing,
                0f
            );
        }

        public Vector3 GetTilePosition(int x, int y)
        {
            return GetCellPosition(x, y);
        }

        public Vector3 GetVerticalWallPosition(int x, int y)
        {
            return GetCellPosition(x, y) + new Vector3(0f, 0f, wallZOffset);
        }

        public Vector3 GetHorizontalWallPosition(int x, int y)
        {
            return GetCellPosition(x, y) + new Vector3(0f, 0f, wallZOffset);
        }

        public Vector3 GetPawnPosition(Position position)
        {
            return GetCellPosition(position.X, position.Y) + new Vector3(0f, 0f, pawnZOffset);
        }
    }
}