using Avalonia.Controls;
using Avalonia.Data;
using Titan.Tools.ManifestBuilder.DataTemplates.Controls;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class StringEditorDescriptor : EditorPropertyDescriptor
{
    public int MaxLength { get; init; } = int.MaxValue; //NOTE(Jens): How should this be implemneted?
    public string? Watermark { get; init; }
    public override IControl Build()
    {
        var panel = new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new TextBlock { Text = $"{Name}:" },
                new TextBox
                {
                    Watermark = Watermark ?? string.Empty,
                    [!TextBox.TextProperty] = new Binding(PropertyName, BindingMode.TwoWay)
                }
            }
        };

        if (Description != null)
        {
            panel.Children.Insert(1, new DescriptionTextBlock(Description));
        }
        return panel;
    }
}
