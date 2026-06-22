using UnityEngine;

namespace Quoridor
{
    public sealed class BoardClickInput : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _raycastLayerMask = ~0;

        private IUserInteractable _hovered;
        private IUserInteractable _pressed;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GetComponent<Camera>();
            }

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (_mainCamera == null)
            {
                Debug.LogWarning("BoardClickInput: Camera is null.");
                return;
            }

            IUserInteractable current = RaycastInteractable2D();

            HandleHover(current);
            HandleMouseDown(current);
            HandleMouseUp(current);
        }

        private IUserInteractable RaycastInteractable2D()
        {
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseWorldPoint = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPoint, _raycastLayerMask);

            if (hit == null) return null;
            return hit.GetComponentInParent<IUserInteractable>();
        }

        private void HandleHover(IUserInteractable current)
        {
            if (_hovered == current) return;
            
            IUserInteractable previous = _hovered;
            _hovered = current;
            
            previous?.MouseOut();
            _hovered?.Hovered();
        }

        private void HandleMouseDown(IUserInteractable current)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (current == null) return;

            _pressed = current;
            _pressed.Pressed();
        }

        private void HandleMouseUp(IUserInteractable current)
        {
            if (!Input.GetMouseButtonUp(0)) return;
            if (_pressed == null) return;

            if (current == _pressed)
            {
                _pressed.Released();
            }
            else
            {
                _pressed.MouseOut();
            }

            _pressed = null;
        }
    }
}