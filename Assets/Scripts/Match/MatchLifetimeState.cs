using System;
using System.Threading;

namespace Quoridor
{
    /// <summary>
    /// Shared lifetime token for every object owned by a match scope.
    /// </summary>
    public sealed class MatchLifetimeState : IDisposable
    {
        private int _isDisposed;

        public bool IsDisposed => Volatile.Read(ref _isDisposed) != 0;

        public void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(MatchSession));
            }
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _isDisposed, 1);
        }
    }
}
