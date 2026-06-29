using System;
using UnityEngine;

namespace Quoridor
{
    public sealed class PawnView : ViewBase
    {
        [SerializeField] private Vector3 _offset = Vector3.zero;

        private PawnViewModel _viewModel;
        private Func<Position, Vector3> _positionResolver;

        public void BindPositionResolver(Func<Position, Vector3> positionResolver)
        {
            _positionResolver = positionResolver;
            ApplyViewModel();
        }

        public void BindViewModel(PawnViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.Changed -= OnViewModelChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.Changed += OnViewModelChanged;
                OnViewModelChanged();
            }
        }

        private void OnViewModelChanged()
        {
            ApplyViewModel();
        }

        private void ApplyViewModel()
        {
            if (_viewModel == null || _positionResolver == null) return;

            Vector3 current = transform.position;
            Vector3 target = _positionResolver(_viewModel.Position) + _offset;

            transform.position = new Vector3(
                target.x,
                target.y,
                current.z
            );
        }

        private void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.Changed -= OnViewModelChanged;
            }
        }
    }
}
