namespace Quoridor
{
    public sealed class MatchInputReleaseValidator
    {
        private readonly InteractionStateStore _interactionStateStore;
        private readonly MatchInputRejectionDispatcher _rejectionDispatcher;

        public MatchInputReleaseValidator(
            InteractionStateStore interactionStateStore,
            MatchInputRejectionDispatcher rejectionDispatcher
        )
        {
            _interactionStateStore = interactionStateStore;
            _rejectionDispatcher = rejectionDispatcher;
        }

        public bool CanAccept(InputTarget target, InputIntent intent)
        {
            if (intent != InputIntent.Released)
            {
                return false;
            }

            if (_interactionStateStore.GetTargetState(target).IsValid)
            {
                return true;
            }

            _rejectionDispatcher.Reject(target, intent, "interaction state judge is invalid");
            return false;
        }
    }
}
