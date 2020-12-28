using System;

// ReSharper disable InconsistentNaming

namespace Titan.ECS.Messaging
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ECSEventAttribute : Attribute { }
}
