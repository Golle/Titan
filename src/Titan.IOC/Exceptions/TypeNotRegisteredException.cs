using System;

namespace Titan.IOC.Exceptions
{
    [Serializable]
    internal class TypeNotRegisteredException : Exception
    {
        public TypeNotRegisteredException(string message) : base(message)
        {
        }
    }
}