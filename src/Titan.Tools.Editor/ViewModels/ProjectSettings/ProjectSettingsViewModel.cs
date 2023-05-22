using Titan.Tools.Editor.Common;

namespace Titan.Tools.Editor.ViewModels.ProjectSettings;

internal interface IProjectSettings
{
    public string Name { get; }
}

internal class ProjectSettingsViewModel : ViewModelBase
{
    public IProjectSettings[] Settings { get; }

    public ProjectSettingsViewModel(IEnumerable<IProjectSettings> projectSettings)
    {
        Settings = projectSettings
            .ToArray();
    }

    public async void OnLoaded()
    {
        foreach (var setting in Settings)
        {
            //await setting.OnLoaded();
        }
    }

    #region DESIGNER
    public ProjectSettingsViewModel()
    : this(new []{ new DesignerSettings() })
    {
        Helper.CheckDesignMode(nameof(ProjectSettingsViewModel));
    }

    private class DesignerSettings : IProjectSettings
    {
        public string Name => "DESIGNER";
    }
    #endregion
}
