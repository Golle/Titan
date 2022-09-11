using System;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class EditorStringAttribute : EditorNodeAttribute
{
    public string? Watermark { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class EditorFileAttribute : EditorNodeAttribute
{
    public string? Watermark { get; set; }
    public string? ButtonText { get; set; }
}
