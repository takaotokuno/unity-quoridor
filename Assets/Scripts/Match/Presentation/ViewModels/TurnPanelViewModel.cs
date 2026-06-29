namespace Quoridor
{
    public sealed class TurnPanelViewModel : ViewModelBase
    {
        private int _currentTurn = 1;
        public int CurrentTurn
        {
            get => _currentTurn;
            set { if (_currentTurn == value) return; _currentTurn = value; OnChanged(); }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set { if (_isVisible == value) return; _isVisible = value; OnChanged(); }
        }

        private string _label = "Turn";
        public string Label
        {
            get => _label;
            set { if (_label == value) return; _label = value; OnChanged(); }
        }
    }
}
