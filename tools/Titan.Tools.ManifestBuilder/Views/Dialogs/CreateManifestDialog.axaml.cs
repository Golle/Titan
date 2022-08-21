using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

namespace Titan.Tools.ManifestBuilder.Views.Dialogs;

public partial class CreateManifestDialog : Window
{
    public CreateManifestDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        DataContext = new CreateManifestDialogViewModel(this);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
