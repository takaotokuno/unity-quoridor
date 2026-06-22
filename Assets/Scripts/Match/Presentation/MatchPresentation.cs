using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchPresentation : IMatchPresentation
    {  
        private readonly ObjectLayoutView _root;
        private readonly MatchControlPresenter _control; 
        private readonly BoardPresenter _board;
        private readonly TurnPanelPresenter _turnPanel;
        private readonly SkillButtonPresenter _skillButton;
        private readonly StatusPanelPresenter _statusPanel;

        private readonly List<IDisposable> _disposables;
        private bool _isDisposed;

        public MatchPresentation(
            ObjectLayoutView root,
            MatchControlPresenter control,
            BoardPresenter board,
            TurnPanelPresenter turnPanel,
            SkillButtonPresenter skillButton,
            StatusPanelPresenter statusPanel
        )
        {
            _root = root;
            _control = control;
            _board = board;
            _turnPanel = turnPanel;
            _skillButton = skillButton;
            _statusPanel = statusPanel;

            _disposables = new List<IDisposable>
            {
                _control,
                _board,
                _turnPanel,
                _skillButton,
                _statusPanel
            };
        }

        public void Dispose()
        {
            if(_isDisposed) return;

            foreach(IDisposable disposable in _disposables)
            {
                disposable.Dispose();    
            }
            UnityEngine.Object.Destroy(_root.gameObject);

            _isDisposed = true;
        }
    }
}
