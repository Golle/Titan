using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.Controls;
using Titan.Tools.ManifestBuilder.Models;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public record struct AddFileToManifestMessage(ManifestItem Item);
public class FileEntryViewModel : ViewModelBase
{
    private bool _selected;
    public string FileName { get; }
    public string FullPath { get; }
    public FileEntryType Type { get; }
    public bool IsDocument => Type is FileEntryType.Document;
    public bool Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public FileEntryViewModel()
    : this("n/a", "n/a", FileEntryType.Document)
    {
    }
    public FileEntryViewModel(string fileName, string fullPath, FileEntryType type)
    {
        FileName = fileName;
        FullPath = fullPath;
        Type = type;
    }
}

public class ContentViewModel : ViewModelBase
{

    private ViewModelBase? _selectedFile;
    public ViewModelBase? SelectedFile
    {
        get => _selectedFile;
        set => SetProperty(ref _selectedFile, value);
    }

    private string? _currentFolder;
    public string? CurrentFolder
    {
        get => _currentFolder;
        set => this.RaiseAndSetIfChanged(ref _currentFolder, value);
    }

    private string? _basePath;
    public string? BasePath
    {
        get => _basePath;
        set => SetProperty(ref _basePath, value);
    }

    public IObservableCollection<FileEntryViewModel> FileEntries { get; } = new ObservableCollectionExtended<FileEntryViewModel>();
    public ICommand Open { get; }
    public ICommand Select { get; }
    public ICommand AddToManifest { get; }

    public ContentViewModel(IMessenger? messenger = null, IManifestItemFactory? manifestItemFactory = null, IDialogService? dialogService = null)
    {
        messenger ??= Registry.GetRequiredService<IMessenger>();
        manifestItemFactory ??= Registry.GetRequiredService<IManifestItemFactory>();
        dialogService ??= Registry.GetRequiredService<IDialogService>();

        Open = ReactiveCommand.Create<FileEntryViewModel>(fileEntry =>
        {
            //NOTE(Jens): should these be tasks?
            if (fileEntry.Type is FileEntryType.Folder or FileEntryType.History)
            {
                CurrentFolder = fileEntry.FullPath;
                FileEntries.Load(EnumerateAll(fileEntry.FullPath));
            }
        });

        Select = ReactiveCommand.Create<FileEntryViewModel>(fileEntry =>
        {
            //NOTE(Jens): should these be tasks?
            SelectedFile = FileInfoViewModel.Create(fileEntry.FullPath);
            foreach (var fileEntryViewModel in FileEntries)
            {
                fileEntryViewModel.Selected = false;
            }
            fileEntry.Selected = true;
        });

        AddToManifest = ReactiveCommand.CreateFromTask<FileEntryViewModel>(async fileEntry =>
        {
            var relativePath = Path.GetRelativePath(_basePath!, fileEntry.FullPath);
            var itemResult = manifestItemFactory.CreateFromPath(relativePath);
            if (itemResult.Failed)
            {
                await dialogService.MessageBox("Error", $"Failed to add file to the manifest with error: {itemResult.Error}");
                return;
            }
            await messenger.SendAsync(new AddFileToManifestMessage(itemResult.Data!));
        });
    }


    public void Load(string path)
    {
        BasePath = path;
        FileEntries.Load(EnumerateAll(path));
    }

    private IEnumerable<FileEntryViewModel> EnumerateAll(string path)
    {
        var isBasePath = BasePath?.Equals(path, StringComparison.InvariantCultureIgnoreCase) ?? throw new InvalidOperationException("BasePath is not set. This is not valid.");
        if (!isBasePath && Directory.GetParent(path) is not null and var parent)
        {
            yield return new FileEntryViewModel("..", parent.FullName, FileEntryType.History);
        }

        foreach (var directory in Directory.EnumerateDirectories(path))
        {
            yield return new FileEntryViewModel(Path.GetFileName(directory), directory, FileEntryType.Folder);
        }
        foreach (var file in Directory.EnumerateFiles(path))
        {
            yield return new FileEntryViewModel(Path.GetFileName(file), file, FileEntryType.Document);
        }
    }

    public ContentViewModel()
        : this(null)
    {

    }
}
