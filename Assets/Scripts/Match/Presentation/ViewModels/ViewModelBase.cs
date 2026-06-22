using System;

namespace Quoridor
{
    public abstract class ViewModelBase
    {
        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set { if (_isValid == value) return; _isValid = value; OnChanged(); }
        }

        private bool _isHovered;
        public bool IsHovered
        {
            get => _isHovered;
            set { if (_isHovered == value) return; _isHovered = value; OnChanged(); }
        }

        private bool _isPressed;
        public bool IsPressed
        {
            get => _isPressed;
            set { if (_isPressed == value) return; _isPressed = value; OnChanged(); }
        }

        public event Action Changed;

        protected void OnChanged() => Changed?.Invoke();
    }
}
