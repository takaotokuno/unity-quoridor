using UnityEngine;

namespace Quoridor
{
    public abstract class BoardCellViewBase : BoardCellViewModelViewBase, IUserInteractable
    {
        private MatchInputPort _inputPort;
        private InputTarget _target;
        private bool _hasTarget;

        public void BindInputPort(MatchInputPort inputPort)
        {
            _inputPort = inputPort;
        }

        public void SetPosition(Position pos)
        {
            _target = CreateInputTarget(pos);
            _hasTarget = true;
        }

        protected abstract InputTarget CreateInputTarget(Position pos);

        public void Hovered() => SendInput(InputIntent.Hovered);
        public void Pressed() => SendInput(InputIntent.Pressed);
        public void Released() => SendInput(InputIntent.Released);
        public void MouseOut() => SendInput(InputIntent.MouseOut);

        private void SendInput(InputIntent intent)
        {
            if (!_hasTarget)
            {
                Debug.LogWarning($"{name}: InputTarget is not initialized. Intent={intent}");
                return;
            }

            if (_inputPort == null)
            {
                Debug.LogWarning($"{name}: MatchInputPort is not bound. Intent={intent}");
                return;
            }

            _inputPort.Handle(_target, intent);
        }

        protected override void ApplyViewModel()
        {
            if (ViewModel.IsPressed && ViewModel.IsValid)
            {
                Press();
            }
            else if (ViewModel.IsHovered && ViewModel.IsValid)
            {
                Highlight();
            }
            else
            {
                Clear();
            }
        }

        public abstract void Highlight();
        public abstract void Emphasize();
        public abstract void Dim();
        public abstract void Press();
        public abstract void Clear();
        public abstract void PlayInvalidFeedback();
    }
}