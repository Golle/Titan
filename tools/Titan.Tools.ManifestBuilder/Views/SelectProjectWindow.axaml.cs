using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels;

namespace Titan.Tools.ManifestBuilder.Views;

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
