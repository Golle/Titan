using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Titan.Tools.ManifestBuilder.Views.Manifests;

public partial class ManifestView : UserControl
{
    public ManifestView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
