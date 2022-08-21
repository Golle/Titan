using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Titan.Tools.ManifestBuilder.Views;

public partial class ManifestView2 : UserControl
{
    public ManifestView2()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
