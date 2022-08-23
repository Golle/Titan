using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

namespace Titan.Tools.ManifestBuilder.Views.Dialogs;

public partial class SelectProjectWindow : Window
{
    public SelectProjectWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        DataContext = new SelectProjectWindowViewModel(this);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
