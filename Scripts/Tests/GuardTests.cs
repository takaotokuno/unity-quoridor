using System;
using NUnit.Framework;
using Quoridor;

namespace Quoridor.Tests
{
    [TestFixture]
    public sealed class GuardTests
    {
        [Test]
        public void ThrowIfNull_ReturnsArgument_WhenArgumentIsNotNull()
        {
            var value = new object();

            object result = Guard.ThrowIfNull(value, nameof(value));

            Assert.AreSame(value, result);
        }

        [Test]
        public void ThrowIfNull_ThrowsArgumentNullException_WithParamName_WhenArgumentIsNull()
        {
            const string paramName = "value";

            var exception = Assert.Throws<ArgumentNullException>(
                () => Guard.ThrowIfNull<object>(null, paramName)
            );

            Assert.AreEqual(paramName, exception.ParamName);
        }
    }
}
