namespace Quoridor
{
    public sealed class MatchFactory
    {
        private int _sessionId;
        private readonly VContainer.IObjectResolver _resolver;

        public MatchFactory(VContainer.IObjectResolver resolver)
        {
            _sessionId = 1;
            _resolver = Guard.ThrowIfNull(resolver, nameof(resolver));
        }

        public MatchLifetimeScope Create(MatchSetting setting)
        {
            Guard.ThrowIfNull(setting, nameof(setting));

            var scope = MatchLifetimeScope.Create(_resolver, _sessionId, setting);
            _sessionId++;
            return scope;
        }
    }
}
