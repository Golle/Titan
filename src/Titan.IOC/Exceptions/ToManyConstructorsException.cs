using System;

namespace Titan.IOC.Exceptions
{
    [Serializable]
    internal class ToManyConstructorsException : Exception
    {
        public ToManyConstructorsException(string message) : base(message)
        {
        }
    }
}