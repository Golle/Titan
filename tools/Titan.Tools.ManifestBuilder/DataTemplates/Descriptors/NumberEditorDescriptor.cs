using Avalonia.Controls;
using Avalonia.Data;
using Titan.Tools.ManifestBuilder.DataTemplates.Controls;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class NumberEditorDescriptor : EditorPropertyDescriptor
{
    public int Min { get; init; }
    public int Max { get; init; }
    public override IControl Build()
    {
        var watermark = (Min, Max) switch
        {
            (int.MinValue, int.MaxValue) => "Any number",
            (int.MinValue, _) => $"Number less than {Max}",
            (_, int.MaxValue) => $"Number greater than {Min}",
            _ => $"Number between {Min} and {Max}"
        };
        var panel = new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new TextBlock { Text = $"{Name}:" },
                new TextBox
                {
                    Watermark = watermark,
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
