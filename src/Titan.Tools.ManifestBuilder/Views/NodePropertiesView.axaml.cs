using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Titan.Tools.ManifestBuilder.Views;

public partial class NodePropertiesView : UserControl
{
    public NodePropertiesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
