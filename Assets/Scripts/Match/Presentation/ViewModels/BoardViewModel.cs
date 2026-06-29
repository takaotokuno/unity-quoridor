namespace Quoridor
{
    public sealed class BoardViewModel : ViewModelBase
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set { if (_isVisible == value) return; _isVisible = value; OnChanged(); }
        }
    }
}
