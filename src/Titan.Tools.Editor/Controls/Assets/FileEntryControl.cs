using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Titan.Tools.Editor.Services.Assets;

namespace Titan.Tools.Editor.Controls.Assets;

public class FileEntryControl : ContentControl
{
    public static readonly StyledProperty<int> IconSizeProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, int>(nameof(IconSize), 40);

    public int IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly StyledProperty<FileEntryType> FileTypeProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, FileEntryType>(nameof(FileType));

    public FileEntryType FileType
    {
        get => GetValue(FileTypeProperty);
        set => SetValue(FileTypeProperty, value);
    }

    public static readonly StyledProperty<string> FileNameProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, string>(nameof(FileType), "[no name]");
    public string FileName
    {
        get => GetValue(FileNameProperty);
        set => SetValue(FileNameProperty, value);
    }

    public static readonly StyledProperty<ICommand?> SelectCommandProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, ICommand?>(nameof(SelectCommand));


    public ICommand? SelectCommand
    {
        get => GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand?> DoubleClickCommandProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, ICommand?>(nameof(DoubleClickCommand));
    public ICommand? DoubleClickCommand
    {
        get => GetValue(DoubleClickCommandProperty);
        set => SetValue(DoubleClickCommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, bool>(nameof(IsSelected), defaultBindingMode: BindingMode.TwoWay);
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        IsSelected = e.ClickCount != 0;
        if (e.ClickCount == 1)
        {
            SelectCommand?.Execute(CommandParameter);
        }
        else if (e.ClickCount >= 2)
        {
            DoubleClickCommand?.Execute(CommandParameter);
        }
    }
}
