using System.Collections.Generic;
using UnityEngine;

namespace Quoridor
{
    public sealed class StatusPanelPresenter 
        : PresenterBase,
          IMatchObserver<InteractionStateChangedEvent>,
          IMatchObserver<DistanceUpdatedEvent>,
          IMatchObserver<StatusAddedEvent>,
          IMatchObserver<StatusRemovedEvent>,
          IEventSubscriber
    {
        private readonly StatusPanelView _panelFirst;
        private readonly StatusPanelViewModel _panelViewModelFirst;
        private readonly StatusPanelView _panelSecond;
        private readonly StatusPanelViewModel _panelViewModelSecond;

        private readonly StatusViewCatalog _statusViewCatalog;
        private readonly StatusIconView _statusViewPrefab;

        private readonly Dictionary<StatusId, StatusIconView> _statusViewsFirst  = new();
        private readonly Dictionary<StatusId, StatusIconView> _statusViewsSecond = new();

        private IMatchEventBus _eventBus;
        private readonly InteractionStateStore _interactionStateStore;

        public StatusPanelPresenter(
            StatusPanelView panelFirst,
            StatusPanelView panelSecond,
            StatusViewCatalog statusViewCatalog,
            StatusIconView statusViewPrefab,
            InteractionStateStore interactionStateStore
        ) : base()
        {
            _panelFirst          = panelFirst;
            _panelViewModelFirst = CreateAndBindModel(_panelFirst);
            _panelSecond         = panelSecond;
            _panelViewModelSecond = CreateAndBindModel(_panelSecond);
            _statusViewCatalog   = statusViewCatalog;
            _statusViewPrefab    = statusViewPrefab;
            _interactionStateStore = interactionStateStore;
        }

        private static StatusPanelViewModel CreateAndBindModel(
            StatusPanelView panelView
        )
        {
            var model = new StatusPanelViewModel();
            panelView.BindViewModel(model);
            return model;
        }
        
        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<InteractionStateChangedEvent>(this);
            _eventBus.Subscribe<DistanceUpdatedEvent>(this);
            _eventBus.Subscribe<StatusAddedEvent>(this);
            _eventBus.Subscribe<StatusRemovedEvent>(this);
        }

        public override void Dispose()
        {
            _eventBus.Unsubscribe<InteractionStateChangedEvent>(this);
            _eventBus.Unsubscribe<DistanceUpdatedEvent>(this);
            _eventBus.Unsubscribe<StatusAddedEvent>(this);
            _eventBus.Unsubscribe<StatusRemovedEvent>(this);

            Object.Destroy(_panelFirst.gameObject);
            Object.Destroy(_panelSecond.gameObject);
        }

        public void Notify(InteractionStateChangedEvent e)
        {
            UpdateRemainWallCount();
        }

        private void UpdateRemainWallCount()
        {
            foreach(PlayerId playerId in PlayerId.All)
            {
                var skillState = _interactionStateStore.GetSkillState(playerId, BuiltInSkillSlotIds.PlaceWall);
                var vm = GetPanelViewModelForPlayer(playerId);
                vm.RemainWallCount = skillState.RemainingUses;
            }
        }

        public void Notify(DistanceUpdatedEvent e)
        {
            UpdateDistance(e.Distances);   
        }

        private void UpdateDistance(DistanceSnapshot distances)
        {
            foreach(PlayerId playerId in PlayerId.All)
            {
                var vm = GetPanelViewModelForPlayer(playerId);
                vm.Distance = distances.GetDistance(playerId);
            }
        }

        public void Notify(StatusAddedEvent e)
        {
            var views = GetViewsForPlayer(e.PlayerId);
            var panel = GetPanelForPlayer(e.PlayerId);
            var entry = _statusViewCatalog.Find(e.StatusId);
            
            StatusIconView view = panel.AddStatusIcon(_statusViewPrefab);

            if (view == null) return;

            if (entry != null)
            {
                view.BindViewDefinition(entry);
            }

            views[e.StatusId] = view;
            view.PlayShow();
        }

        public void Notify(StatusRemovedEvent e)
        {
            // イベントが来た時点で重ねがけが全て解消されたことが保証されている
            var views = GetViewsForPlayer(e.PlayerId);
            var panel = GetPanelForPlayer(e.PlayerId);

            if (!views.TryGetValue(e.StatusId, out StatusIconView view))
            {
                return;
            }

            views.Remove(e.StatusId);
            panel.RemoveStatusIcon(view);
        }

        private Dictionary<StatusId, StatusIconView> GetViewsForPlayer(PlayerId playerId)
        {
            return playerId.IsFirstPlayer
                ? _statusViewsFirst
                : _statusViewsSecond;
        }

        private StatusPanelView GetPanelForPlayer(PlayerId playerId)
        {
            return playerId.IsFirstPlayer
                ? _panelFirst
                : _panelSecond;
        }

        private StatusPanelViewModel GetPanelViewModelForPlayer(PlayerId playerId)
        {
            return playerId.IsFirstPlayer 
                ? _panelViewModelFirst
                : _panelViewModelSecond;
        }
    }
}
