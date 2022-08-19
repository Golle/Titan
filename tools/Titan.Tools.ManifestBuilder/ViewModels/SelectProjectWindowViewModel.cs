using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class RecentProjectViewModel
{
    public required string Title { get; init; }
    public required string Path { get; init; }

    public static RecentProjectViewModel Create(string path) =>
        new()
        {
            Title = System.IO.Path.GetFileNameWithoutExtension(path) ?? "unknown",
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
    public SelectProjectWindowViewModel(Window parentWindow)
        : this(parentWindow, null)
    {
    }

    public SelectProjectWindowViewModel(Window parentWindow, IAppSettings? appSettings = null)
    {
        appSettings ??= Registry.GetRequiredService<IAppSettings>();
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
            var result = await dialog.ShowAsync(parentWindow);
            if (result != null)
            {
                var settings = appSettings.GetSettings();
                if (settings.RecentProjects.AddProject(result))
                {
                    appSettings.Save(settings);
                }
                parentWindow.Close(result);
            }
        });

        OpenRecentProject = ReactiveCommand.Create<string>(path =>
        {
            if (!Directory.Exists(path))
            {
                // do some error handling and remove the project from the list.
                return;
            }
            parentWindow.Close(path);
        });
    }
}
