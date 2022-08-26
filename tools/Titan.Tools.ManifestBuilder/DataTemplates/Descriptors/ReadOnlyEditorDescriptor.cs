using Avalonia.Controls;
using Avalonia.Data;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class ReadOnlyEditorDescriptor : EditorPropertyDescriptor
{
    public override IControl Build() =>
        new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new TextBlock { Text = $"{Name}:" },
                new TextBlock { [!TextBlock.TextProperty] = new Binding(PropertyName, BindingMode.OneTime)}
            }
        };
}
