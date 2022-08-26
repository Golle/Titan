using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Titan.Tools.ManifestBuilder.Controls;

public enum FileEntryType
{
    Document,
    Folder,
    History
}


public class FileEntryControl : ContentControl
{
    public static readonly StyledProperty<string> FileNameProperty =
        AvaloniaProperty.Register<FileEntryControl, string>(nameof(FileName));

    public static readonly StyledProperty<ICommand?> DoubleClickProperty =
        AvaloniaProperty.Register<FileEntryControl, ICommand?>(nameof(DoubleClick));

    public static readonly StyledProperty<ICommand?> SingleClickProperty =
        AvaloniaProperty.Register<FileEntryControl, ICommand?>(nameof(SingleClick));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<FileEntryControl, object>(nameof(CommandParameter));

    public static readonly StyledProperty<FileEntryType> FileEntryTypeProperty =
        AvaloniaProperty.Register<FileEntryControl, FileEntryType>(nameof(FileEntryType), defaultValue: FileEntryType.Document);

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<FileEntryControl, bool>(nameof(IsSelected));
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    public FileEntryType FileEntryType
    {
        get => GetValue(FileEntryTypeProperty);
        set => SetValue(FileEntryTypeProperty, value);
    }
    public string FileName
    {
        get => GetValue(FileNameProperty);
        set => SetValue(FileNameProperty, value);
    }

    public ICommand? DoubleClick
    {
        get => GetValue(DoubleClickProperty);
        set => SetValue(DoubleClickProperty, value);
    }

    public ICommand? SingleClick
    {
        get => GetValue(SingleClickProperty);
        set => SetValue(SingleClickProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        switch (e.ClickCount)
        {
            case 1:
                Execute(SingleClick, CommandParameter);
                break;
            case 2:
                Execute(DoubleClick, CommandParameter);
                break;

        }

        static void Execute(ICommand? command, object? parameter)
        {
            var canExecute = command?.CanExecute(parameter) ?? false;
            if (canExecute)
            {
                command?.Execute(parameter);
            }
        }
    }
}
