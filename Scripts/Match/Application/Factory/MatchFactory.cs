namespace Quoridor
{
    public sealed class MatchFactory
    {
        private int _sessionId;
        private MatchStateFactory _stateFactory;
        private readonly MatchCommandPortFactory _commandPortFactory;
        private readonly CpuAgentFactory _cpuAgentFactory;
        private readonly MatchPresentationFactory _presentationFactory;
        private readonly ISoundService _sound;
        private readonly ITimeEffectService _timeEffect;
        private readonly INovelGamePort _novel;
        private readonly IBackgroundEffectService _background;
        private readonly IGameLogger _gameLogger;

        public MatchFactory(
            MatchStateFactory stateFactory,
            MatchCommandPortFactory commandPortFactory,
            CpuAgentFactory cpuAgentFactory,
            MatchPresentationFactory presentationFactory,
            ISoundService sound,
            ITimeEffectService timeEffect,
            INovelGamePort novel,
            IBackgroundEffectService background,
            IGameLogger gameLogger
        )
        {
            _sessionId = 1;
            _stateFactory = stateFactory;
            _commandPortFactory = commandPortFactory;
            _cpuAgentFactory = cpuAgentFactory;
            _presentationFactory = presentationFactory;
            _sound = sound;
            _timeEffect = timeEffect;
            _novel = novel;
            _background = background;
            _gameLogger = gameLogger;
        }

        public MatchSession Create(MatchSetting setting)
        {
            var stateConfig = MatchConfigMapper.ToStateConfig(setting);
            var presentationConfig = MatchConfigMapper.ToPresentationConfig(setting);

            var state = _stateFactory.Create(stateConfig);
            
            var eventBus = new MatchEventBus(); 
            var interpreter = new MatchEventInterpreter(
                _sound,
                _timeEffect,
                _novel,
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

            // PlayerConfig.IsCpu == true のプレイヤーに対して CPU エージェントを生成・初期化する
            _cpuAgentFactory.Create(
                stateConfig,
                state,
                commandPort,
                eventBus
            );
            
            var presentation = _presentationFactory.Create(
                presentationConfig, 
                state, 
                eventBus, 
                commandPort
            );

            var session = new MatchSession(
                _sessionId, 
                commandPort, 
                eventBus, 
                presentation
            );
            _sessionId ++;
            eventBus.DispatchEvent(new MatchReadiedEvent());

            return session;
        }
    }
}