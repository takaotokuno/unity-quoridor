using System;

namespace Quoridor
{
    public static class Guard
    {
        public static T ThrowIfNull<T>(T argument, string paramName) where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(paramName);

            return argument;
        }
    }
}
