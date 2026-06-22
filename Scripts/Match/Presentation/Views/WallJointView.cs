using UnityEngine;

namespace Quoridor
{
    public sealed class WallJointView : BoardCellViewModelViewBase
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private Color _normalColor = new Color(0.45f, 0.3f, 0.15f, 1f);
        [SerializeField] private Color _highlightColor = new Color(0.8f, 0.65f, 0.3f, 1f);
        [SerializeField] private Color _emphasizeColor = new Color(0.95f, 0.8f, 0.35f, 1f);
        [SerializeField] private Color _dimColor = new Color(0.25f, 0.2f, 0.15f, 1f);

        [SerializeField] private Vector3 _highlightScale = new Vector3(1.03f, 1.03f, 1.03f);
        [SerializeField] private Vector3 _emphasizeScale = new Vector3(1.08f, 1.08f, 1.08f);

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

        protected override void ApplyViewModel()
        {
            if (ViewModel == null) return;

            if (ViewModel.IsBuilt)
            {
                Show();
            }
            else
            {
                Hide();
                return;
            }

            if (ViewModel.IsHovered && ViewModel.IsValid)
            {
                Highlight();
            }
            else
            {
                Clear();
            }
        }

        public void Highlight()
        {
            SetColor(_highlightColor);
            SetScale(_highlightScale);
        }

        public void Emphasize()
        {
            SetColor(_emphasizeColor);
            SetScale(_emphasizeScale);
        }

        public void Dim()
        {
            SetColor(_dimColor);
            SetScale(_initialScale);
        }

        public void Clear()
        {
            SetColor(_normalColor);
            SetScale(_initialScale);
        }

        private void SetColor(Color color)
        {
            SetColor(_spriteRenderer, color);
        }
    }
}