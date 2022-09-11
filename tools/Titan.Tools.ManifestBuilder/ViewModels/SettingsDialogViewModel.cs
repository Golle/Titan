using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.DataTemplates;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class SettingsViewModel : ViewModelBase, IPropertyEditor
{
    public SettingsViewModel(Settings settings)
    {
        Path = settings.EditorPath;
        ArgumentsFormat = settings.EditorArgumentsFormat;
    }

    [EditorFile(Watermark = "Editor path", ButtonText = "Browse")]
    [DisplayName("Editor executable path")]
    [Description("The absolute path to your editor of choice, for example Visual Studio Code")]
    public string? Path { get; set; }

    [EditorString(Watermark = "Editor arguments")]
    [DisplayName("Editor arguments format")]
    [Description("Use %d for folder and %f for the current file.")]
    public string? ArgumentsFormat { get; set; }
}

public class SettingsDialogViewModel : ViewModelBase
{
    public SettingsViewModel Settings { get; }
    public ICommand SaveChanges { get; }
    public SettingsDialogViewModel(Window window, IAppSettings? appSettings = null)
    {
        appSettings ??= Registry.GetRequiredService<IAppSettings>();

        Settings = new SettingsViewModel(appSettings.GetSettings());
        SaveChanges = ReactiveCommand.Create(() =>
        {
            var settings = appSettings.GetSettings();
            appSettings.Save(settings with
            {
                EditorArgumentsFormat = Settings.ArgumentsFormat,
                EditorPath = Settings.Path
            });
            window.Close();
        });
    }

    public SettingsDialogViewModel()
    : this(null!)
    {
    }
}
