using System;
// ReSharper disable InconsistentNaming

namespace Titan.ECS.Events
{
    [AttributeUsage(AttributeTargets.Struct)]
    internal class ECSEventAttribute : Attribute{}
}
