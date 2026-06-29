using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchObjects : IMatchObjects
    {  
        private readonly ObjectLayoutView _root;
        private readonly MatchControlPresenter _control; 
        private readonly BoardPresenter _board;
        private readonly TurnPanelPresenter _turnPanel;
        private readonly SkillButtonPresenter _skillButton;
        private readonly StatusPanelPresenter _statusPanel;

        private readonly List<IDisposable> _disposables;
        private bool _isDisposed;

        public MatchObjects(
            ObjectLayoutView root,
            IReadOnlyList<CpuAgent> cpuAgents,
            MatchControlPresenter control,
            BoardPresenter board,
            TurnPanelPresenter turnPanel,
            SkillButtonPresenter skillButton,
            StatusPanelPresenter statusPanel
        )
        {
            Guard.ThrowIfNull(cpuAgents, nameof(cpuAgents));

            _root = Guard.ThrowIfNull(root, nameof(root));
            _control = Guard.ThrowIfNull(control, nameof(control));
            _board = Guard.ThrowIfNull(board, nameof(board));
            _turnPanel = Guard.ThrowIfNull(turnPanel, nameof(turnPanel));
            _skillButton = Guard.ThrowIfNull(skillButton, nameof(skillButton));
            _statusPanel = Guard.ThrowIfNull(statusPanel, nameof(statusPanel));

            _disposables = new List<IDisposable>
            {
                _control,
                _board,
                _turnPanel,
                _skillButton,
                _statusPanel
            };

            foreach (var cpuAgent in cpuAgents)
            {
                _disposables.Add(cpuAgent);
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            foreach (IDisposable disposable in _disposables)
            {
                disposable.Dispose();
            }
            UnityEngine.Object.Destroy(_root.gameObject);

            _isDisposed = true;
        }
    }
}
