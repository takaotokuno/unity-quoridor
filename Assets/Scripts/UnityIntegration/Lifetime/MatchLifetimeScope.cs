using System;
using VContainer;

namespace Quoridor
{
    /// <summary>
    /// Owns the child DI scope and all runtime objects for a single match.
    /// </summary>
    public sealed class MatchLifetimeScope : IDisposable
    {
        private readonly IScopedObjectResolver _resolver;
        private bool _disposed;

        public MatchSession Session { get; }

        private MatchLifetimeScope(
            IScopedObjectResolver resolver,
            MatchSession session
        )
        {
            _resolver = Guard.ThrowIfNull(resolver, nameof(resolver));
            Session = Guard.ThrowIfNull(session, nameof(session));
        }

        public static MatchLifetimeScope Create(
            IObjectResolver parentResolver,
            int sessionId,
            MatchSetting setting
        )
        {
            Guard.ThrowIfNull(parentResolver, nameof(parentResolver));
            Guard.ThrowIfNull(setting, nameof(setting));

            MatchStateConfig stateConfig = MatchConfigMapper.ToStateConfig(setting);
            MatchObjectsConfig objectsConfig = MatchConfigMapper.ToObjectsConfig(setting);

            IScopedObjectResolver resolver = parentResolver.CreateScope(builder =>
            {
                builder.RegisterInstance(stateConfig);
                builder.RegisterInstance(objectsConfig);
                builder.RegisterInstance(sessionId);

                builder.Register<MatchState>(container =>
                    container.Resolve<MatchStateFactory>().Create(
                        container.Resolve<MatchStateConfig>()
                    ), Lifetime.Scoped);

                builder.Register<IMatchEventBus, MatchEventBus>(Lifetime.Scoped);
                builder.Register<MatchEventInterpreter>(Lifetime.Scoped);
                builder.Register<MatchEventLogObserver>(Lifetime.Scoped);

                builder.Register<IMatchCommandPort>(container =>
                    container.Resolve<MatchCommandPortFactory>().Create(
                        container.Resolve<MatchState>(),
                        container.Resolve<IMatchEventBus>()
                    ), Lifetime.Scoped);

                builder.Register<IMatchObjects>(container =>
                    container.Resolve<MatchObjectsFactory>().Create(
                        container.Resolve<MatchObjectsConfig>(),
                        container.Resolve<MatchStateConfig>(),
                        container.Resolve<MatchState>(),
                        container.Resolve<IMatchEventBus>(),
                        container.Resolve<IMatchCommandPort>()
                    ), Lifetime.Scoped);

                builder.Register<MatchSession>(Lifetime.Scoped);
            });

            try
            {
                IMatchEventBus eventBus = resolver.Resolve<IMatchEventBus>();
                resolver.Resolve<MatchEventInterpreter>();
                resolver.Resolve<MatchEventLogObserver>();

                // Materialize the presentation graph in this scope. Its IDisposable
                // lifetime is owned and released by the scoped resolver.
                resolver.Resolve<IMatchObjects>();
                MatchSession session = resolver.Resolve<MatchSession>();
                eventBus.DispatchEvent(new MatchReadiedEvent());
                return new MatchLifetimeScope(resolver, session);
            }
            catch
            {
                resolver.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _resolver.Dispose();
            _disposed = true;
        }
    }
}
