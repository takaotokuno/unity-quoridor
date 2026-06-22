namespace Quoridor
{
    public abstract class BoardCellViewModelViewBase : ViewBase
    {
        protected BoardCellViewModel ViewModel { get; private set; }

        public void BindViewModel(BoardCellViewModel viewModel)
        {
            if (ViewModel != null)
            {
                ViewModel.Changed -= OnViewModelChangedInternal;
            }

            ViewModel = viewModel;

            if (ViewModel != null)
            {
                ViewModel.Changed += OnViewModelChangedInternal;
                OnViewModelChangedInternal();
            }
        }

        private void OnViewModelChangedInternal()
        {
            if (ViewModel == null) return;

            OnBeforeViewModelChanged();
            ApplyViewModel();
            OnAfterViewModelChanged();
        }

        protected virtual void OnBeforeViewModelChanged()
        {
        }

        protected virtual void OnAfterViewModelChanged()
        {
        }

        protected abstract void ApplyViewModel();

        protected virtual void OnDestroy()
        {
            if (ViewModel != null)
            {
                ViewModel.Changed -= OnViewModelChangedInternal;
            }
        }
    }
}