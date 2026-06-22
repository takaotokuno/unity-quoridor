using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchHistory
    {
        private readonly List<MatchMemento> _history;
        private int _current;

        public bool CanUndo => _current > 0;
        public bool CanRedo => _current >= 0 && _current < _history.Count - 1;

        public MatchHistory(MatchMemento initial)
        {
            Guard.ThrowIfNull(initial, nameof(initial));

            _history = new List<MatchMemento>();
            _current = -1;

            Push(initial);
        }

        public void Push(MatchMemento memento)
        {
            Guard.ThrowIfNull(memento, nameof(memento));

            // Redo履歴を破棄
            if (_current < _history.Count - 1)
            {
                _history.RemoveRange(
                    _current + 1,
                    _history.Count - _current - 1
                );
            }

            _history.Add(memento);
            _current++;
        }

        public bool TryUndo(out MatchMemento memento)
        {
            if (!CanUndo)
            {
                memento = null;
                return false;
            }

            _current--;
            memento = _history[_current];
            return true;
        }

        public bool TryRedo(out MatchMemento memento)
        {
            if (!CanRedo)
            {
                memento = null;
                return false;
            }

            _current++;
            memento = _history[_current];
            return true;
        }
    }
}
