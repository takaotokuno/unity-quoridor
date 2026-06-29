namespace Quoridor
{
    public sealed class BoardView : ViewBase
    {
        private BoardViewModel _viewModel;

        public void BindViewModel(BoardViewModel viewModel)
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
            if (_viewModel == null) return;

            if (_viewModel.IsVisible)
            {
                PlayShow();
            }
            else
            {
                PlayHide();
            }
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
