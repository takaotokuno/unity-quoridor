namespace Quoridor
{
    public sealed class MatchInputPort
    {
        private readonly MatchInputStateUpdater _stateUpdater;
        private readonly MatchInputReleaseValidator _releaseValidator;
        private readonly MatchInputCommandDispatcher _commandDispatcher;

        public MatchInputPort(
            MatchInputStateUpdater stateUpdater,
            MatchInputReleaseValidator releaseValidator,
            MatchInputCommandDispatcher commandDispatcher
        )
        {
            _stateUpdater = stateUpdater;
            _releaseValidator = releaseValidator;
            _commandDispatcher = commandDispatcher;
        }

        public void Handle(InputTarget target, InputIntent intent)
        {
            _stateUpdater.Apply(target, intent);

            if (!_releaseValidator.CanAccept(target, intent))
            {
                return;
            }

            _commandDispatcher.Dispatch(target);
        }
    }
}
