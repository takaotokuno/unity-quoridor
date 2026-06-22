namespace Quoridor
{
    public sealed class MatchInputRejectionDispatcher
    {
        private readonly IMatchEventBus _eventBus;

        public MatchInputRejectionDispatcher(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Reject(InputTarget target, InputIntent intent, string reason)
        {
            _eventBus.DispatchEvent<InputRejectedEvent>(new(target, intent, reason));
        }
    }
}
