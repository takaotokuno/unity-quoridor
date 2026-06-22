namespace Quoridor
{
    public sealed class MatchInputStateUpdater
    {
        private readonly InputStateStore _inputStateStore;
        private readonly IMatchEventBus _eventBus;

        public MatchInputStateUpdater(
            InputStateStore inputStateStore,
            IMatchEventBus eventBus
        )
        {
            _inputStateStore = inputStateStore;
            _eventBus = eventBus;
        }

        public void Apply(InputTarget target, InputIntent intent)
        {
            _inputStateStore.ApplyIntent(target, intent);
            _eventBus.DispatchEvent<InputReceivedEvent>(new(target, intent));
        }
    }
}
