using System;
using System.Linq;
using NUnit.Framework;

namespace Quoridor.Tests
{
    public sealed class MatchSessionTests
    {
        [Test]
        public void MatchSession_IsANonDisposableFacade()
        {
            Assert.That(typeof(IDisposable).IsAssignableFrom(typeof(MatchSession)), Is.False);
            Assert.That(
                typeof(MatchSession).GetConstructors().Single().GetParameters()
                    .Select(parameter => parameter.ParameterType),
                Is.EquivalentTo(new[]
                {
                    typeof(int),
                    typeof(IMatchCommandPort),
                    typeof(IMatchEventBus)
                })
            );
        }

        [Test]
        public void DispatchCommand_DelegatesToCommandPort()
        {
            var commandPort = new RecordingCommandPort();
            var session = new MatchSession(42, commandPort, new StubEventBus());

            IMatchResponse response = session.DispatchCommand(null);

            Assert.That(session.SessionId, Is.EqualTo(42));
            Assert.That(response, Is.SameAs(commandPort.Response));
            Assert.That(commandPort.DispatchCount, Is.EqualTo(1));
        }

        private sealed class RecordingCommandPort : IMatchCommandPort
        {
            public IMatchResponse Response { get; } = new StubMatchResponse();
            public int DispatchCount { get; private set; }

            public IMatchResponse DispatchCommand(IMatchCommand command)
            {
                DispatchCount++;
                return Response;
            }
        }

        private sealed class StubEventBus : IMatchEventBus
        {
            public void Subscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent { }
            public void Unsubscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent { }
            public void DispatchEvent<T>(T e) where T : IMatchEvent { }
        }

        private sealed class StubMatchResponse : IMatchResponse
        {
            public bool IsSuccess => true;
            public string Message => string.Empty;
        }
    }
}
