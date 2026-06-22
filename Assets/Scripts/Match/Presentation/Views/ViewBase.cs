using UnityEngine;

namespace Quoridor
{
    public class ViewBase : MonoBehaviour
    {
        [Header("Common View")]
        [SerializeField] protected Transform _visualRoot;

        protected Vector3 _initialScale;

        protected virtual void Awake()
        {
            InitializeBase();
            OnInitialize();
        }

        protected virtual void InitializeBase()
        {
            if (_visualRoot == null)
            {
                _visualRoot = transform;
            }
            _initialScale = _visualRoot.localScale;
        }

        protected virtual void OnInitialize()
        {
        }

        protected Color GetCurrentColor(SpriteRenderer spriteRenderer, Color fallback)
        {
            if (spriteRenderer != null)
            {
                return spriteRenderer.color;
            }

            return fallback;
        }

        protected void SetColor(SpriteRenderer spriteRenderer, Color color)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }

        protected void SetScale(Vector3 scale)
        {
            if (_visualRoot != null)
            {
                _visualRoot.localScale = scale;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void PlayShow()
        {
            Show();
        }

        public virtual void PlayHide()
        {
            Hide();
        }
    }
}