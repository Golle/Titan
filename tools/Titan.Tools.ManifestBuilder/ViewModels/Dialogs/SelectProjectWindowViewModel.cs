using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

public class RecentProjectViewModel
{
    public required string Title { get; init; }
    public required string Path { get; init; }
    public static RecentProjectViewModel Create(string path) =>
        new()
        {
            Title = System.IO.Path.GetFileNameWithoutExtension(path),
            Path = path
        };
}
public class SelectProjectWindowViewModel : ViewModelBase
{
    public ICommand CloseWindow { get; }
    public ICommand OpenProject { get; }
    public ICommand OpenRecentProject { get; }
    public IEnumerable<RecentProjectViewModel> RecentProjects { get; }

    public SelectProjectWindowViewModel() : this(null!) { }
    public SelectProjectWindowViewModel(Window parentWindow, IAppSettings? appSettings = null, IApplicationState? applicationState = null)
    {
        appSettings ??= Registry.GetRequiredService<IAppSettings>();
        applicationState ??= Registry.GetRequiredService<IApplicationState>();
        RecentProjects = appSettings
            .GetSettings()
            .RecentProjects
            .Paths
            .Select(RecentProjectViewModel.Create)
            .ToArray();

        CloseWindow = ReactiveCommand.Create(() => parentWindow.Close(null));
        OpenProject = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new OpenFolderDialog();
            var projectPath = await dialog.ShowAsync(parentWindow);
            if (projectPath != null)
            {
                var settings = appSettings.GetSettings();
                if (settings.RecentProjects.AddProject(projectPath))
                {
                    appSettings.Save(settings);
                }
                parentWindow.Close(projectPath);

                applicationState.ProjectPath = projectPath;
            }
        });

        OpenRecentProject = ReactiveCommand.Create<string>(projectPath =>
        {
            if (!Directory.Exists(projectPath))
            {
                // do some error handling and remove the project from the list.
                return;
            }
            parentWindow.Close(projectPath);
            applicationState.ProjectPath = projectPath;
        });
    }
}
