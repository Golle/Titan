using System;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

public class CreateManifestDialogViewModel : ViewModelBase
{
    private string? _name;
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ICommand Close { get; }
    public ICommand Create { get; }

    public CreateManifestDialogViewModel(Window window)
    {
        Close = ReactiveCommand.Create(() => window.Close(null));
        Create = ReactiveCommand.Create(() => window.Close(_name));
    }

    public CreateManifestDialogViewModel()
        : this(null!)
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This ctor is for the designer.");
        }
    }
}
