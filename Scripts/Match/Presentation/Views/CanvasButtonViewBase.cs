using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quoridor
{
    [RequireComponent(typeof(Image))]
    public abstract class CanvasButtonViewBase : ViewBase, IUserInteractable,
        IPointerEnterHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {
        private MatchInputPort _inputPort;
        private InputTarget _target;
        private ButtonViewModel _viewModel;

        protected abstract ButtonId ButtonId { get; }

        public void BindInputPort(MatchInputPort inputPort)
        {
            _inputPort = inputPort;
            _target = InputTarget.Button(ButtonId);
        }

        public void BindViewModel(ButtonViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.Changed -= OnViewModelChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.Changed += OnViewModelChanged;
                OnViewModelChanged();
            }
        }

        private void OnViewModelChanged()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (_viewModel.IsVisible)
            {
                Show();
            }
            else
            {
                Hide();
                return;
            }

            if (_viewModel.IsDimmed)
            {
                Dim();
            }
            else if (_viewModel.IsPressed && _viewModel.IsValid)
            {
                Press();
            }
            else if (_viewModel.IsHovered && _viewModel.IsValid)
            {
                Highlight();
            }
            else
            {
                Clear();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Pressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Released();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOut();
        }

        public void Hovered()
        {
            if (_inputPort == null)
            {
                Debug.LogWarning($"{GetType().Name}: InputPort is not bound.", this);
                return;
            }

            _inputPort.Handle(_target, InputIntent.Hovered);
        }

        public void Pressed()
        {
            if (_inputPort == null)
            {
                Debug.LogWarning($"{GetType().Name}: InputPort is not bound.", this);
                return;
            }

            _inputPort.Handle(_target, InputIntent.Pressed);
        }

        public void Released()
        {
            if (_inputPort == null)
            {
                Debug.LogWarning($"{GetType().Name}: InputPort is not bound.", this);
                return;
            }

            _inputPort.Handle(_target, InputIntent.Released);
        }

        public void MouseOut()
        {
            if (_inputPort == null)
            {
                Debug.LogWarning($"{GetType().Name}: InputPort is not bound.", this);
                return;
            }

            _inputPort.Handle(_target, InputIntent.MouseOut);
        }

        public virtual void Highlight()
        {
        }

        public virtual void Emphasize()
        {
        }

        public virtual void Dim()
        {
        }

        public virtual void Press()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void PlayInvalidFeedback()
        {
        }
    }
}