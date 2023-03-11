using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

namespace Titan.Tools.ManifestBuilder.Views.Dialogs;

public partial class MessageBoxDialog : Window
{
    public MessageBoxDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static MessageBoxDialog Create(string title, string? message, MessageBoxType type)
    {
        var dialog = new MessageBoxDialog();
        dialog.DataContext = new MessageBoxViewModel(dialog, title, message ?? string.Empty, type);
        return dialog;
    }
}
