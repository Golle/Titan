using System;
using Avalonia.Controls;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal abstract class EditorPropertyDescriptor
{
    public int Order { get; set; }
    public required string Name { get; set; }
    public required string PropertyName { get; set; }
    public required string? Description { get; set; }
    public required Func<object?> Accessor { get; set; }
    public abstract IControl Build();
}
