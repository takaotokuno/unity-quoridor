using System.Collections;
using UnityEngine;

namespace Quoridor
{
    public sealed class WallView : BoardCellViewBase
    {
        [Header("Hit Areas")]
        [SerializeField] private GameObject _hitAreaObject;      // 未設置時の当たり判定
        [SerializeField] private GameObject _wallHitAreaObject;  // 設置後の当たり判定

        [Header("Renderers")]
        [SerializeField] private SpriteRenderer _hitAreaSpriteRenderer; // 未設置時の見た目
        [SerializeField] private SpriteRenderer _wallSpriteRenderer;    // 設置後の見た目

        [Header("Hit Area Colors")]
        [SerializeField] private Color _normalHitAreaColor = new Color(1f, 1f, 1f, 0.08f);
        [SerializeField] private Color _highlightHitAreaColor = new Color(0.6f, 1f, 0.6f, 0.35f);
        [SerializeField] private Color _emphasizeHitAreaColor = new Color(1f, 0.95f, 0.4f, 0.5f);
        [SerializeField] private Color _dimHitAreaColor = new Color(0.4f, 0.4f, 0.4f, 0.05f);
        [SerializeField] private Color _pressedHitAreaColor = new Color(0.7f, 0.85f, 1f, 0.45f);

        [Header("Wall Colors")]
        [SerializeField] private Color _normalWallColor = new Color(0.45f, 0.3f, 0.15f, 1f);
        [SerializeField] private Color _highlightWallColor = new Color(0.8f, 0.65f, 0.3f, 1f);
        [SerializeField] private Color _emphasizeWallColor = new Color(0.95f, 0.8f, 0.35f, 1f);
        [SerializeField] private Color _dimWallColor = new Color(0.25f, 0.2f, 0.15f, 1f);

        [Header("Common Colors")]
        [SerializeField] private Color _invalidColor = new Color(1f, 0.3f, 0.3f, 0.8f);

        [Header("Scales")]
        [SerializeField] private Vector3 _highlightScale = new Vector3(1.03f, 1.03f, 1.03f);
        [SerializeField] private Vector3 _emphasizeScale = new Vector3(1.08f, 1.08f, 1.08f);
        [SerializeField] private Vector3 _pressedScale = new Vector3(0.97f, 0.97f, 0.97f);

        [Header("Invalid Feedback")]
        [SerializeField] private float _invalidFlashInterval = 0.08f;
        [SerializeField] private int _invalidFlashCount = 2;

        private bool _isBuilt;
        private Coroutine _invalidFeedbackCoroutine;

        protected override void OnInitialize()
        {
            InitializeNormalColors();

            ApplyBuiltState();
            ApplyVisualByCurrentState();
        }

        protected override InputTarget CreateInputTarget(Position pos)
        {
            return InputTarget.Wall(pos);
        }

        protected override void OnBeforeViewModelChanged()
        {
            if (ViewModel == null) return;

            StopInvalidFeedbackIfNeeded();

            if (_isBuilt != ViewModel.IsBuilt)
            {
                _isBuilt = ViewModel.IsBuilt;
                ApplyBuiltState();
            }
        }

        public void PlaceWall()
        {
            StopInvalidFeedbackIfNeeded();

            _isBuilt = true;
            ApplyVisualByCurrentState();
        }

        public void RemoveWall()
        {
            StopInvalidFeedbackIfNeeded();

            _isBuilt = false;
            ApplyVisualByCurrentState();
        }

        public override void Highlight()
        {
            StopInvalidFeedbackIfNeeded();

            if (_isBuilt)
            {
                SetWallColor(_highlightWallColor);
            }
            else
            {
                SetHitAreaColor(_highlightHitAreaColor);
            }

            SetScale(_highlightScale);
        }

        public override void Emphasize()
        {
            StopInvalidFeedbackIfNeeded();

            if (_isBuilt)
            {
                SetWallColor(_emphasizeWallColor);
            }
            else
            {
                SetHitAreaColor(_emphasizeHitAreaColor);
            }

            SetScale(_emphasizeScale);
        }

        public override void Dim()
        {
            StopInvalidFeedbackIfNeeded();

            if (_isBuilt)
            {
                SetWallColor(_dimWallColor);
            }
            else
            {
                SetHitAreaColor(_dimHitAreaColor);
            }

            SetScale(_initialScale);
        }

        public override void Press()
        {
            StopInvalidFeedbackIfNeeded();

            if (_isBuilt)
            {
                SetWallColor(_highlightWallColor);
            }
            else
            {
                SetHitAreaColor(_pressedHitAreaColor);
            }

            SetScale(_pressedScale);
        }

        public override void Clear()
        {
            StopInvalidFeedbackIfNeeded();
            ApplyVisualByCurrentState();
        }

        public override void PlayInvalidFeedback()
        {
            StopInvalidFeedbackIfNeeded();
            _invalidFeedbackCoroutine = StartCoroutine(PlayInvalidFeedbackCoroutine());
        }

        private void InitializeNormalColors()
        {
            _normalHitAreaColor = GetCurrentColor(
                _hitAreaSpriteRenderer,
                _normalHitAreaColor
            );

            _normalWallColor = GetCurrentColor(
                _wallSpriteRenderer,
                _normalWallColor
            );
        }

        private void ApplyBuiltState()
        {
            if (_hitAreaObject != null)
            {
                _hitAreaObject.SetActive(!_isBuilt);
            }

            if (_wallHitAreaObject != null)
            {
                _wallHitAreaObject.SetActive(_isBuilt);
            }

            if (_hitAreaSpriteRenderer != null)
            {
                _hitAreaSpriteRenderer.gameObject.SetActive(!_isBuilt);
            }

            if (_wallSpriteRenderer != null)
            {
                _wallSpriteRenderer.gameObject.SetActive(_isBuilt);
            }
        }

        private void ApplyVisualByCurrentState()
        {
            SetScale(_initialScale);

            if (_isBuilt)
            {
                SetWallColor(_normalWallColor);
            }
            else
            {
                SetHitAreaColor(_normalHitAreaColor);
            }
        }

        private void SetHitAreaColor(Color color)
        {
            SetColor(_hitAreaSpriteRenderer, color);
        }

        private void SetWallColor(Color color)
        {
            SetColor(_wallSpriteRenderer, color);
        }

        private void StopInvalidFeedbackIfNeeded()
        {
            if (_invalidFeedbackCoroutine == null) return;

            StopCoroutine(_invalidFeedbackCoroutine);
            _invalidFeedbackCoroutine = null;
        }

        private IEnumerator PlayInvalidFeedbackCoroutine()
        {
            for (int i = 0; i < _invalidFlashCount; i++)
            {
                if (_isBuilt)
                {
                    SetWallColor(_invalidColor);
                }
                else
                {
                    SetHitAreaColor(_invalidColor);
                }

                SetScale(_emphasizeScale);
                yield return new WaitForSeconds(_invalidFlashInterval);

                ApplyVisualByCurrentState();
                yield return new WaitForSeconds(_invalidFlashInterval);
            }

            _invalidFeedbackCoroutine = null;
        }
    }
}