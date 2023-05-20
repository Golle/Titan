using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Services.Assets;
using Titan.Tools.Editor.Services.State;

namespace Titan.Tools.Editor.ViewModels;

public partial class AssetBrowserViewModel : ViewModelBase
{
    private readonly IContentBrowser _contentBrowser;
    private readonly IApplicationState _applicationState;

    [ObservableProperty]
    private string? _currentFolder;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private ObservableCollection<FileEntry> _files = new();

    public bool IsEmpty => Files.Count == 0;

    public AssetBrowserViewModel(IContentBrowser contentBrowser, IApplicationState applicationState)
    {
        _contentBrowser = contentBrowser;
        _applicationState = applicationState;
    }

    public async Task LoadContents()
    {
        CurrentFolder ??= _applicationState.AssetsDirectory;
        Files.Clear();

        if (!IsRootFolder())
        {
            Files.Add(new FileEntry
            {
                Path = string.Empty,
                Type = FileEntryType.ParentFolder,
                Name = ".."
            });
        }

        var contents = await _contentBrowser.GetFiles(CurrentFolder);
        foreach (var fileEntry in contents)
        {
            Files.Add(fileEntry);
        }
    }

    [RelayCommand]
    private async Task OpenDirectory(FileEntry fileEntry)
    {
        if (fileEntry.Type is FileEntryType.Folder)
        {
            CurrentFolder = Path.Combine(CurrentFolder!, fileEntry.Path);
            await LoadContents();
        }

        if (fileEntry.Type is FileEntryType.ParentFolder && !IsRootFolder())
        {
            CurrentFolder = Directory.GetParent(CurrentFolder!)!.FullName;
            await LoadContents();
        }
    }

    private bool IsRootFolder() => Path.GetRelativePath(_applicationState.AssetsDirectory, CurrentFolder!) == ".";

    #region DESIGNER

    public AssetBrowserViewModel()
        : this(App.GetRequiredService<IContentBrowser>(),
            App.GetRequiredService<IApplicationState>())
    {
        Helper.CheckDesignMode(nameof(AssetBrowserViewModel));
    }

    #endregion
}
