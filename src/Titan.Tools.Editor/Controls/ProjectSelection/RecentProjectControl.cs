using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Titan.Tools.Editor.Controls.ProjectSelection;

public class RecentProjectControl : ContentControl
{
    public static readonly StyledProperty<string> ProjectNameProperty = AvaloniaProperty.Register<RecentProjectControl, string>(nameof(ProjectName), "n/a");
    public string ProjectName
    {
        get => GetValue(ProjectNameProperty);
        set => SetValue(ProjectNameProperty, value);
    }

    public static readonly StyledProperty<string> PathProperty = AvaloniaProperty.Register<RecentProjectControl, string>(nameof(Path), "n/a");
    public string Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public static readonly StyledProperty<string> LastOpenedProperty = AvaloniaProperty.Register<RecentProjectControl, string>(nameof(LastOpened));
    public string LastOpened
    {
        get => GetValue(LastOpenedProperty);
        set => SetValue(LastOpenedProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<RecentProjectControl, ICommand?>(nameof(Command));
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<RecentProjectControl, object?>(nameof(CommandParameter));
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly StyledProperty<ICommand?> RemoveCommandProperty = AvaloniaProperty.Register<RecentProjectControl, ICommand?>(nameof(RemoveCommand));
    public ICommand? RemoveCommand
    {
        get => GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (Command == null)
        {
            return;
        }
        if (Command.CanExecute(CommandParameter))
        {
            Command?.Execute(CommandParameter);
        }
    }
}
