using System;
using NUnit.Framework;

namespace Quoridor.Tests
{
    public sealed class MatchLifetimeStateTests
    {
        [Test]
        public void Dispose_IsIdempotentAndMarksLifetimeAsDisposed()
        {
            var lifetime = new MatchLifetimeState();

            lifetime.Dispose();
            lifetime.Dispose();

            Assert.That(lifetime.IsDisposed, Is.True);
            Assert.Throws<ObjectDisposedException>(() => lifetime.ThrowIfDisposed());
        }
    }
}
