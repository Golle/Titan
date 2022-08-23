using System;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

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