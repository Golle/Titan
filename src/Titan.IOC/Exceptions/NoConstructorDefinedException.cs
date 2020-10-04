using System;

namespace Titan.IOC.Exceptions
{
    [Serializable]
    internal class NoConstructorDefinedException : Exception
    {
        public NoConstructorDefinedException(string message)
            : base(message)
        {
        }
    }
}
