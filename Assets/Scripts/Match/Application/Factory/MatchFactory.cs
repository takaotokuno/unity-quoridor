namespace Quoridor
{
    public sealed class MatchFactory
    {
        private int _sessionId;
        private MatchStateFactory _stateFactory;
        private readonly MatchCommandPortFactory _commandPortFactory;
        private readonly MatchObjectsFactory _matchObjectsFactory;
        private readonly ISoundService _sound;
        private readonly ITimeEffectService _timeEffect;
        private readonly IBackgroundEffectService _background;
        private readonly IGameLogger _gameLogger;

        public MatchFactory(
            MatchStateFactory stateFactory,
            MatchCommandPortFactory commandPortFactory,
            MatchObjectsFactory matchObjectsFactory,
            ISoundService sound,
            ITimeEffectService timeEffect,
            IBackgroundEffectService background,
            IGameLogger gameLogger
        )
        {
            _sessionId = 1;
            _stateFactory = stateFactory;
            _commandPortFactory = commandPortFactory;
            _matchObjectsFactory = matchObjectsFactory;
            _sound = sound;
            _timeEffect = timeEffect;
            _background = background;
            _gameLogger = gameLogger;
        }

        public MatchSession Create(MatchSetting setting)
        {
            var stateConfig = MatchConfigMapper.ToStateConfig(setting);
            var objectsConfig = MatchConfigMapper.ToObjectsConfig(setting);

            var state = _stateFactory.Create(stateConfig);

            var eventBus = new MatchEventBus();
            var interpreter = new MatchEventInterpreter(
                _sound,
                _timeEffect,
                _background
            );
            interpreter.SubscribeTo(eventBus);

            var logObserver = new MatchEventLogObserver(
                _gameLogger,
                state
            );
            logObserver.SubscribeTo(eventBus);

            var commandPort = _commandPortFactory.Create(
                state,
                eventBus
            );

            var matchObjects = _matchObjectsFactory.Create(
                objectsConfig,
                stateConfig,
                state,
                eventBus,
                commandPort
            );

            var session = new MatchSession(
                _sessionId,
                commandPort,
                eventBus,
                matchObjects
            );
            _sessionId ++;
            eventBus.DispatchEvent(new MatchReadiedEvent());

            return session;
        }
    }
}
