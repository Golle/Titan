using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Titan.Tools.ManifestBuilder.Models;

public class Manifest
{
    [JsonIgnore]
    public string? Path { get; set; }
    public required string Name { get; set; }
    public List<TextureItem> Textures { get; set; } = new();
    public List<ModelItem> Models { get; set; } = new();
}

public class TextureItem : ManifestItem
{
    public required string Path { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextureType Type { get; set; }
}

public class ModelItem : ManifestItem
{
    public required string Path { get; set; }
}

public class ManifestItem
{
    [EditorString]
    public required string Name { get; set; }
}

public enum TextureType
{
    Unknown,
    PNG,
    BMP,
    JPG
}

[AttributeUsage(AttributeTargets.Property)]
public class EditorNumberAttribute : EditorNodeAttribute
{
    public int Min { get; }
    public int Max { get; }
    public EditorNumberAttribute(int min = int.MinValue, int max = int.MaxValue)
    {
        if (min > max)
        {
            throw new InvalidOperationException("The min value must be less than the max value");
        }

        Min = min;
        Max = max;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class EditorStringAttribute : EditorNodeAttribute
{
    public string? Watermark { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class EditorReadOnlyAttribute : EditorNodeAttribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public abstract class EditorNodeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class EditorEnumAttribute : EditorNodeAttribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class OrderAttribute : Attribute
{
    public int Order { get; }
    public OrderAttribute(int order)
    {
        Order = order;
    }
}
