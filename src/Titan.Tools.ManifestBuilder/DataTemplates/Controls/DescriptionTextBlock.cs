using Avalonia;
using Avalonia.Controls;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Controls;

internal class DescriptionTextBlock : TextBlock
{
    public DescriptionTextBlock(string description)
    {
        Text = description;
        FontSize = 10;
        Margin = new Thickness(4, 0, 0, 0);
        Opacity = 0.7;
    }
}
