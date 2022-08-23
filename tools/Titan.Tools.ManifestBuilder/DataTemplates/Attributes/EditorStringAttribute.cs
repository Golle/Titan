using System;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class EditorStringAttribute : EditorNodeAttribute
{
    public string? Watermark { get; set; }
}