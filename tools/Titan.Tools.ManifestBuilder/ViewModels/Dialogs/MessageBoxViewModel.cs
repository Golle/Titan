using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

public enum MessageBoxType
{
    Ok,
    OkCancel,
    YesNo
}

public enum MessageBoxResult
{
    Cancel = 0,
    Ok = 1,
    Yes = Ok,
    No = Cancel
}

public class MessageBoxViewModel : ViewModelBase
{
    public string Title { get; }
    public string Message { get; }

    public bool IsOk { get; }
    public bool IsOkCancel { get; }
    public bool IsYesNo { get; }

    public ICommand Close { get; }
    public MessageBoxViewModel(Window window, string title, string message, MessageBoxType type)
    {
        Title = title;
        Message = message;
        IsOk = type is MessageBoxType.Ok or MessageBoxType.OkCancel;
        IsOkCancel = type is MessageBoxType.OkCancel;
        IsYesNo = type is MessageBoxType.YesNo;
        Close = ReactiveCommand.Create<MessageBoxResult>(result => window.Close(result));
    }


    public MessageBoxViewModel()
    : this(null!, "Design", "Fake message", MessageBoxType.OkCancel)
    {
    }
}

