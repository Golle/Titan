using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.DataTemplates;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class SettingsViewModel : ViewModelBase, IPropertyEditor
{
    private readonly Settings _settings;

    public SettingsViewModel(Settings settings)
    {
        _settings = settings;
    }

    [EditorString]
    public string? Test { get; set; }

    [EditorFile(Watermark = "Path to your editor")]
    public string? Path { get; set; }
}

public class SettingsDialogViewModel : ViewModelBase
{

    public SettingsViewModel Settings { get; }
    public ICommand SaveChanges { get; }
    public SettingsDialogViewModel(IAppSettings? appSettings)
    {
        appSettings ??= Registry.GetRequiredService<IAppSettings>();

        Settings = new SettingsViewModel(appSettings.GetSettings());
        SaveChanges = ReactiveCommand.CreateFromTask(() => Task.Delay(1000));
    }

    public SettingsDialogViewModel()
    : this(null)
    {

    }
}
