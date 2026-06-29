using System.Collections.Generic;

namespace Quoridor
{
    /// <summary>
    /// 同点時ランダム選択を含む「最大スコアの候補群」管理。
    /// Greedy と αβ root の最善手更新ロジックを共通化する。
    /// </summary>
    public sealed class CpuBestCommandAccumulator
    {
        private readonly IRandomProvider _randomProvider;
        private readonly List<IMatchCommand> _bestCommands = new();

        public bool HasCommand => _bestCommands.Count > 0;
        public int BestScore { get; private set; } = int.MinValue;

        public CpuBestCommandAccumulator(IRandomProvider randomProvider)
        {
            _randomProvider = Guard.ThrowIfNull(randomProvider, nameof(randomProvider));
        }

        public void Add(IMatchCommand command, int score)
        {
            Guard.ThrowIfNull(command, nameof(command));

            if (score > BestScore)
            {
                BestScore = score;
                _bestCommands.Clear();
                _bestCommands.Add(command);
                return;
            }

            if (score == BestScore)
            {
                _bestCommands.Add(command);
            }
        }

        public CpuBestCommandSelection Select()
        {
            if (_bestCommands.Count == 0)
                return CpuBestCommandSelection.Empty();

            int index = _randomProvider.Range(0, _bestCommands.Count);
            return CpuBestCommandSelection.Success(_bestCommands[index], BestScore);
        }
    }
}
