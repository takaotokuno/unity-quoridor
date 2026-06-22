using UnityEngine;

namespace Quoridor
{
    public sealed class TurnPanelPresenter 
        : PresenterBase,
          IMatchObserver<MatchReadiedEvent>, 
          IMatchObserver<TurnStartedEvent>,
          IEventSubscriber
    {
        private readonly TurnPanelView _panel;
        private IMatchEventBus _eventBus;
        public TurnPanelPresenter(TurnPanelView panel)
            : base()
        {
            _panel = panel;   
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchReadiedEvent>(this);
            _eventBus.Subscribe<TurnStartedEvent>(this);
        }

        public override void Dispose()
        {
            _eventBus.Unsubscribe<MatchReadiedEvent>(this);
            _eventBus.Unsubscribe<TurnStartedEvent>(this);

            Object.Destroy(_panel);
        }

        public void Notify(MatchReadiedEvent e)
        {
            _panel.PlayShow();
        }

        public void Notify(TurnStartedEvent e)
        {
            _panel.UpdateTurnCount(e.CurrentTurn);
        }
    }
}