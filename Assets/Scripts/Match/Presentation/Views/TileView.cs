using UnityEngine;

namespace Quoridor
{
    public sealed class TileView : BoardCellViewBase
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _highlightColor = new Color(0.8f, 1f, 0.8f);
        [SerializeField] private Color _emphasizeColor = new Color(1f, 0.95f, 0.4f);
        [SerializeField] private Color _dimColor = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color _pressedColor = new Color(0.75f, 0.85f, 1f);
        [SerializeField] private Color _invalidColor = new Color(1f, 0.35f, 0.35f);

        [SerializeField] private Vector3 _highlightScale = new Vector3(1.05f, 1.05f, 1.05f);
        [SerializeField] private Vector3 _emphasizeScale = new Vector3(1.12f, 1.12f, 1.12f);
        [SerializeField] private Vector3 _pressedScale = new Vector3(0.96f, 0.96f, 0.96f);

        protected override void OnInitialize()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (_spriteRenderer != null)
            {
                _normalColor = _spriteRenderer.color;
            }
        }

        protected override InputTarget CreateInputTarget(Position pos)
        {
            return InputTarget.Tile(pos);
        }

        public override void Highlight()
        {
            SetColor(_highlightColor);
            SetScale(_highlightScale);
        }

        public override void Emphasize()
        {
            SetColor(_emphasizeColor);
            SetScale(_emphasizeScale);
        }

        public override void Dim()
        {
            SetColor(_dimColor);
            SetScale(_initialScale);
        }

        public override void Press()
        {
            SetColor(_pressedColor);
            SetScale(_pressedScale);
        }

        public override void Clear()
        {
            SetColor(_normalColor);
            SetScale(_initialScale);
        }

        public override void PlayInvalidFeedback()
        {
            SetColor(_invalidColor);
            SetScale(_emphasizeScale);
        }

        private void SetColor(Color color)
        {
            SetColor(_spriteRenderer, color);
        }
    }
}