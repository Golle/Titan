using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

namespace Titan.Tools.ManifestBuilder.Views.Dialogs;

public partial class CookAssetsDialog : Window
{
    public CookAssetsDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        DataContext = new CookAssetsViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
