using Avalonia.Controls;
using Titan.Tools.Editor.ViewModels.Dialogs;

namespace Titan.Tools.Editor.Views.Dialogs;

public partial class MessageBoxDialog : Window
{
    public MessageBoxDialog()
        : this("n/a", "design", MessageBoxType.Ok)
    {
    }

    public MessageBoxDialog(string title, string message, MessageBoxType type)
    {
        InitializeComponent();
        DataContext = new MessageBoxViewModel(title, message, type)
        {
            Window = this
        };
    }
}
