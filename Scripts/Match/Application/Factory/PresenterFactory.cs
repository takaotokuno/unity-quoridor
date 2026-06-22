using UnityEngine;

namespace Quoridor
{
    public sealed partial class PresenterFactory : IPresenterFactory
    {
        private readonly MatchPresentationConfig _setting;
        private MatchViewPrefabCatalog presenterSetting => _setting.ViewPrefabCatalog;
        private ObjectLayoutView layout => _setting.ObjectLayoutView;
        private readonly MatchInputPort _inputPort;
        private readonly IMatchEventBus _eventBus;
        private readonly InteractionStateStore _interactionStateStore;
        private readonly InputStateStore _inputStateStore;
        private readonly SkillSelectionStore _skillSelectionStore;
        private readonly SkillViewCatalog _skillViewCatalog;
        private readonly StatusViewCatalog _statusViewCatalog;

        public PresenterFactory(
            MatchPresentationConfig setting,
            MatchInputPort inputPort, 
            IMatchEventBus eventBus,
            InteractionStateStore interactionStateStore,
            InputStateStore inputStateStore,
            SkillSelectionStore skillSelectionStore,
            SkillViewCatalog skillViewCatalog,
            StatusViewCatalog statusViewCatalog
        )
        {
            _setting = setting;
            _inputPort = inputPort;
            _eventBus = eventBus;
            _interactionStateStore = interactionStateStore;
            _inputStateStore = inputStateStore;
            _skillSelectionStore = skillSelectionStore;
            _skillViewCatalog = skillViewCatalog;
            _statusViewCatalog = statusViewCatalog;
        }

        public MatchControlPresenter CreateControl()
        {
            CanvasView canvas = layout.CanvasView;
            MatchControlView controlView = layout.MatchControlView; 

            Vector3 resignWorldPos = layout.ResignButton.transform.position;
            Vector3 resignLocalPos = canvas.transform.InverseTransformPoint(resignWorldPos);
            ResignButtonView resignButton = _CreateView(presenterSetting.ResignButtonPrefab, resignLocalPos, controlView.transform);
            Object.Destroy(layout.ResignButton.gameObject);

            Vector3 skipWorldPos = layout.SkipButton.transform.position;
            Vector3 skipLocalPos = canvas.transform.InverseTransformPoint(skipWorldPos);
            SkipButtonView   skipButton   = _CreateView(presenterSetting.SkipButtonPrefab, skipLocalPos, controlView.transform);
            Object.Destroy(layout.SkipButton.gameObject);

            resignButton.BindInputPort(_inputPort);
            skipButton.BindInputPort(_inputPort);

            var buttonViewModels = new ButtonViewModel[]
            {
                new(), // index 0 = ButtonId 1 (Resign)
                new(), // index 1 = ButtonId 2 (Skip)
            };

            resignButton.BindViewModel(buttonViewModels[0]);
            skipButton.BindViewModel(buttonViewModels[1]);

            var buttonViews = new IUserInteractable[]
            {
                resignButton,
                skipButton,
            };

            MatchControlPresenter presenter = new(
                controlView,
                buttonViews,
                buttonViewModels,
                _interactionStateStore,
                _inputStateStore
            );

            presenter.SubscribeTo(_eventBus);
            return presenter;
        }

        public TurnPanelPresenter CreateTurnPanel()
        {
            CanvasView canvas = layout.CanvasView;
            Vector3 worldPos = layout.TurnPanel.transform.position;
            Vector3 localPos = canvas.transform.InverseTransformPoint(worldPos);
            
            TurnPanelView turnPanel = _CreateView(presenterSetting.TurnPanelPrefab, localPos, canvas.transform);
            Object.Destroy(layout.TurnPanel.gameObject);
            
            TurnPanelPresenter presenter = new(turnPanel);
            presenter.SubscribeTo(_eventBus);
            return presenter;
        }

        public StatusPanelPresenter CreateStatusPanel()
        {
            CanvasView canvas = layout.CanvasView;

            Vector3 worldPosFirst = layout.StatusPanelFirst.transform.position;
            Vector3 localPosFirst = canvas.transform.InverseTransformPoint(worldPosFirst);
            StatusPanelView panelFirst  = _CreateView(presenterSetting.StatusPanelPrefab, localPosFirst, canvas.transform);
            Object.Destroy(layout.StatusPanelFirst.gameObject);

            Vector3 worldPosSecond = layout.StatusPanelSecond.transform.position;
            Vector3 localPosSecond = canvas.transform.InverseTransformPoint(worldPosSecond);
            StatusPanelView panelSecond = _CreateView(presenterSetting.StatusPanelPrefab, localPosSecond, canvas.transform);
            Object.Destroy(layout.StatusPanelSecond.gameObject);

            StatusPanelPresenter presenter = new(
                panelFirst,
                panelSecond,
                _statusViewCatalog, 
                presenterSetting.StatusPrefab,
                _interactionStateStore
            );

            presenter.SubscribeTo(_eventBus);

            return presenter;
        }

        private T _CreateView<T>(T prefab, Vector3 localPosition, Transform parent) where T : ViewBase
        {
            T view = Object.Instantiate(prefab, parent);
            view.transform.localPosition = localPosition;
            view.transform.localRotation = Quaternion.identity;
            return view;
        }

        private T _CreateView<T>(T prefab, Vector3 worldPosition) where T : ViewBase
        {
            T view = Object.Instantiate(prefab);
            view.transform.position = worldPosition;
            view.transform.rotation = Quaternion.identity;
            return view;
        }
    }   
}