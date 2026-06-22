namespace Quoridor
{
    public class SkillButtonViewModel : ButtonViewModel
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected == value) return; _isSelected = value; OnChanged(); }
        }

        private int _remainingUses;
        public int RemainingUses
        {
            get => _remainingUses;
            set { if (_remainingUses == value) return; _remainingUses = value; OnChanged(); }
        }
    }
}