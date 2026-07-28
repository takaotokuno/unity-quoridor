using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using NUnit.Framework;

namespace Quoridor.Tests
{
    public sealed class MatchCommandPortTests
    {
        [Test]
        public void Dispose_ClearsPendingCommandsAndInvalidatesPostedCallback()
        {
            var context = new RecordingSynchronizationContext();
            var lifetime = new MatchLifetimeState();
            var port = CreatePort(context, lifetime);

            port.DispatchCommand(new UndoCommand("test"));
            port.Dispose();

            Assert.That(GetPendingCommandCount(port), Is.Zero);
            Assert.DoesNotThrow(() => context.InvokePostedCallback());
            Assert.That(context.PostCount, Is.EqualTo(1));
        }

        [Test]
        public void DispatchCommand_ThrowsAfterPortIsDisposed()
        {
            var port = CreatePort(
                new RecordingSynchronizationContext(),
                new MatchLifetimeState()
            );

            port.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                port.DispatchCommand(new UndoCommand("test"))
            );
        }

        [Test]
        public void DispatchCommand_ThrowsAfterMatchLifetimeIsDisposed()
        {
            var lifetime = new MatchLifetimeState();
            var port = CreatePort(new RecordingSynchronizationContext(), lifetime);

            lifetime.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                port.DispatchCommand(new UndoCommand("test"))
            );
        }

        private static MatchCommandPort CreatePort(
            SynchronizationContext context,
            MatchLifetimeState lifetime
        )
        {
            var executor = (MatchCommandExecutor)FormatterServices.GetUninitializedObject(
                typeof(MatchCommandExecutor)
            );
            return new MatchCommandPort(executor, context, new StubLogger(), lifetime);
        }

        private static int GetPendingCommandCount(MatchCommandPort port)
        {
            FieldInfo field = typeof(MatchCommandPort).GetField(
                "_queue",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            var queue = (ICollection)field.GetValue(port);
            return queue.Count;
        }

        private sealed class RecordingSynchronizationContext : SynchronizationContext
        {
            private SendOrPostCallback _callback;
            private object _state;

            public int PostCount { get; private set; }

            public override void Post(SendOrPostCallback d, object state)
            {
                PostCount++;
                _callback = d;
                _state = state;
            }

            public void InvokePostedCallback()
            {
                _callback(_state);
            }
        }

        private sealed class StubLogger : IGameLogger
        {
            public void Log(string message) { }
            public void Warning(string message) { }
            public void Error(string message) { }
        }
    }
}
