using Avalonia.Controls;
using Avalonia.Data;
using Titan.Tools.ManifestBuilder.DataTemplates.Controls;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class ReadOnlyEditorDescriptor : EditorPropertyDescriptor
{
    public override IControl Build()
    {
        var panel = new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new TextBlock { Text = $"{Name}:" },
                new TextBlock { [!TextBlock.TextProperty] = new Binding(PropertyName, BindingMode.OneTime) }
            }
        };

        if (Description != null)
        {
            panel.Children.Insert(1, new DescriptionTextBlock(Description));

        }
        return panel;
    }
}
