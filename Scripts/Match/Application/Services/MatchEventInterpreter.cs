namespace Quoridor
{
    public sealed class MatchEventInterpreter 
        : IMatchObserver<PawnMovedEvent>,
          IMatchObserver<WallPlacedEvent>, 
          IMatchObserver<MatchFinishedEvent>,
          IEventSubscriber
    {
        private IMatchEventBus _eventBus;
        private readonly ISoundService _sound;
        private readonly ITimeEffectService _timeEffect;
        private readonly INovelGamePort _novel;
        private readonly IBackgroundEffectService _background;

        public MatchEventInterpreter(
            ISoundService sound,
            ITimeEffectService timeEffect,
            INovelGamePort novel,
            IBackgroundEffectService background
        )
        {
            _sound = sound;
            _timeEffect = timeEffect;
            _novel = novel;
            _background = background;
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
            _eventBus = eventBus;
            _eventBus.Subscribe<PawnMovedEvent>(this);
            _eventBus.Subscribe<WallPlacedEvent>(this);
            _eventBus.Subscribe<MatchFinishedEvent>(this);
        }
    }
}