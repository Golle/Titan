using System;

namespace Titan.Core.Messaging
{
    /// <summary>
    /// The TitanEventAttribute is used for marking an Event type so the scanner can find it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    internal class TitanEventAttribute : Attribute
    {
    }
}
