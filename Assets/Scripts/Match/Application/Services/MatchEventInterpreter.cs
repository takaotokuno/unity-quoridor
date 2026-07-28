using System;

namespace Quoridor
{
    public sealed class MatchEventInterpreter 
        : IMatchObserver<PawnMovedEvent>,
          IMatchObserver<WallPlacedEvent>, 
          IMatchObserver<MatchFinishedEvent>,
          IEventSubscriber,
          IDisposable
    {
        private IMatchEventBus _eventBus;
        private readonly ISoundService _sound;
        private readonly ITimeEffectService _timeEffect;
        private readonly IBackgroundEffectService _background;

        public MatchEventInterpreter(
            IMatchEventBus eventBus,
            ISoundService sound,
            ITimeEffectService timeEffect,
            IBackgroundEffectService background
        )
        {
            _sound = sound;
            _timeEffect = timeEffect;
            _background = background;
            SubscribeTo(eventBus);
        }

        public void Notify(PawnMovedEvent e)
        {
            _sound.PlaySe(SeId.MovePawn);
        }

        public void Notify(WallPlacedEvent e)
        {
            _sound.PlaySe(SeId.PlaceWall);            
        }

        public void Notify(MatchFinishedEvent e)
        {
            _timeEffect.ApplyHitStop(1, 1);
            _sound.PlaySe(SeId.VictoryJingle); 
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            Guard.ThrowIfNull(eventBus, nameof(eventBus));
            if (_eventBus != null) return;

            _eventBus = eventBus;
            _eventBus.Subscribe<PawnMovedEvent>(this);
            _eventBus.Subscribe<WallPlacedEvent>(this);
            _eventBus.Subscribe<MatchFinishedEvent>(this);
        }

        public void Dispose()
        {
            if (_eventBus == null) return;

            _eventBus.Unsubscribe<PawnMovedEvent>(this);
            _eventBus.Unsubscribe<WallPlacedEvent>(this);
            _eventBus.Unsubscribe<MatchFinishedEvent>(this);
            _eventBus = null;
        }
    }
}
