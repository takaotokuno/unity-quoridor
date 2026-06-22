using System;

namespace Quoridor
{
    public sealed class StatusPanelViewModel
    {
        private int _remainWallCount = 0;
        public int RemainWallCount
        {
            get => _remainWallCount;
            set { if (_remainWallCount == value) return; _remainWallCount = value; OnChangedValue(); }
        }

        private int _distance = 999;
        public int Distance
        {
            get => _distance;
            set { if (_distance == value) return; _distance = value; OnChangedValue();}
        }

        public event Action ChangedValue;

        private void OnChangedValue() => ChangedValue?.Invoke();
    }
}

