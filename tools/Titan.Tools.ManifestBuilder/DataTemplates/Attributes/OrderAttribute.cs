using System;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class OrderAttribute : Attribute
{
    public int Order { get; }
    public OrderAttribute(int order)
    {
        Order = order;
    }
}