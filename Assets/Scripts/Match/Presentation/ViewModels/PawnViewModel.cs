namespace Quoridor
{
    public sealed class PawnViewModel : ViewModelBase
    {
        private Position _position;
        public Position Position
        {
            get => _position;
            set
            {
                if (_position.X == value.X && _position.Y == value.Y) return;

                _position = value;
                OnChanged();
            }
        }

        public PawnViewModel(Position position)
        {
            _position = position;
        }
    }
}
