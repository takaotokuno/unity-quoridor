using UnityEngine;
using TMPro;

namespace Quoridor
{
    public sealed class StatusPanelView : ViewBase
    {
        [Header("Panel Elements")]
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private TMP_Text _wallCountText;
        [SerializeField] private StatusIconRootView _iconRoot;

        public StatusPanelViewModel ViewModel { get; private set; }

        public void BindViewModel(StatusPanelViewModel viewModel)
        {
            if (ViewModel != null)
            {
                ViewModel.ChangedValue -= OnViewModelChangedInternal;
            }

            ViewModel = viewModel;

            if (ViewModel != null)
            {
                ViewModel.ChangedValue += OnViewModelChangedInternal;
                OnViewModelChangedInternal();
            }
        }

        private void OnViewModelChangedInternal()
        {
            if (ViewModel == null) return;
            ApplyViewModelValue();
        }

        public StatusIconView AddStatusIcon(StatusIconView prefab, StatusViewEntry entry)
        {
            if (_iconRoot == null)
            {
                Debug.LogError($"{nameof(StatusPanelView)}: IconRoot is not assigned.");
                return null;
            }

            if (prefab == null)
            {
                Debug.LogError($"{nameof(StatusPanelView)}: StatusView prefab is null.");
                return null;
            }

            StatusIconView view = Object.Instantiate(prefab, _iconRoot.transform);

            if (entry != null)
            {
                view.BindViewDefinition(entry);
            }

            view.PlayShow();
            return view;
        }

        public void RemoveStatusIcon(StatusIconView view)
        {
            if (view == null)
            {
                return;
            }

            view.PlayHide(() =>
            {
                if (view != null)
                {
                    Object.Destroy(view.gameObject);
                }
            });
        }

        private void ApplyViewModelValue()
        {
            if (ViewModel == null) return;
            _distanceText.text = ViewModel.Distance.ToString();
            _wallCountText.text = ViewModel.RemainWallCount.ToString();
        }
    }    
}
