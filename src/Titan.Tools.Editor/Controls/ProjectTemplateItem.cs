using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;

namespace Titan.Tools.Editor.Controls;

public class ProjectTemplateItem : ContentControl
{
    public static readonly StyledProperty<string> TemplateNameProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, string>(nameof(TemplateName));

    public static readonly StyledProperty<Bitmap?> IconProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, Bitmap?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, bool>(nameof(IsSelected));

    public static readonly StyledProperty<ICommand?> OnSelectProperty =
        AvaloniaProperty.Register<ProjectTemplateItem, ICommand?>(nameof(OnSelect));

    public string TemplateName
    {
        get => GetValue(TemplateNameProperty);
        set => SetValue(TemplateNameProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public Bitmap? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand? OnSelect
    {
        get => GetValue(OnSelectProperty);
        set => SetValue(OnSelectProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (OnSelect == null)
        {
            return;
        }
        if (OnSelect.CanExecute(TemplateName))
        {
            OnSelect.Execute(TemplateName);
        }
    }
}
