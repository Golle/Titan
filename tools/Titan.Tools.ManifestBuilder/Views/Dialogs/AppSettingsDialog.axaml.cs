using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels;

namespace Titan.Tools.ManifestBuilder.Views.Dialogs;

public partial class AppSettingsDialog : Window
{
    public AppSettingsDialog()
    {
        InitializeComponent();
        DataContext = new SettingsDialogViewModel();
    }   

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
