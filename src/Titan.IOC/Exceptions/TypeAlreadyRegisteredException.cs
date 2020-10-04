using System;

namespace Titan.IOC.Exceptions
{
    [Serializable]
    internal class TypeAlreadyRegisteredException : Exception
    {
        public TypeAlreadyRegisteredException(string message) : base(message)
        {
        }
    }
}