using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class EnumEditorDescriptor : EditorPropertyDescriptor
{
    public required Array Values { get; init; }
    public required string TypeName { get; init; }
    public override IControl Build() =>
        new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new TextBlock { Text = $"{Name} - ({TypeName})" },
                new ComboBox
                {
                    Items = Values,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    [!SelectingItemsControl.SelectedItemProperty] = new Binding(PropertyName, BindingMode.TwoWay)
                }
            }
        };
}
