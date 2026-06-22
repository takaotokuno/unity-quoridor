namespace Quoridor
{
    public class ButtonViewModel : ViewModelBase
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set { if (_isVisible == value) return; _isVisible = value; OnChanged(); }
        }

        private bool _isDimmed;
        public bool IsDimmed
        {
            get => _isDimmed;
            set { if (_isDimmed == value) return; _isDimmed = value; OnChanged(); }
        }
    }
}
