using System;
using NUnit.Framework;

namespace Quoridor.Tests
{
    public sealed class MatchSessionIdTests
    {
        [Test]
        public void Constructor_WithPositiveValue_CreatesId()
        {
            var sessionId = new MatchSessionId(42);

            Assert.That(sessionId.Value, Is.EqualTo(42));
            Assert.That(sessionId.ToString(), Is.EqualTo("42"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WithNonPositiveValue_Throws(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MatchSessionId(value));
        }

        [Test]
        public void Next_ReturnsFollowingIdWithoutChangingCurrentId()
        {
            var sessionId = new MatchSessionId(42);

            MatchSessionId nextSessionId = sessionId.Next();

            Assert.That(sessionId.Value, Is.EqualTo(42));
            Assert.That(nextSessionId.Value, Is.EqualTo(43));
        }
    }
}
