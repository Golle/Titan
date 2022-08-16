using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Avalonia.Media;
using DynamicData.Binding;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Controls;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class FileEntryModel
{
    public string FileName { get; }
    public string FullPath { get; }
    public FileEntryType Type { get; }
    public FileEntryModel()
    : this("n/a", "n/a", FileEntryType.Document)
    {
    }
    public FileEntryModel(string fileName, string fullPath, FileEntryType type)
    {
        FileName = fileName;
        FullPath = fullPath;
        Type = type;
    }
};

public class ContentViewModel : ViewModelBase
{
    private string? _info;
    public string? Info
    {
        get => _info;
        set => this.RaiseAndSetIfChanged(ref _info, value);
    }

    private string? _currentFolder;

    public string? CurrentFolder
    {
        get => _currentFolder;
        set => this.RaiseAndSetIfChanged(ref _currentFolder, value);
    }

    public string BasePath { get; }
    public IObservableCollection<FileEntryModel> FileEntries { get; } = new ObservableCollectionExtended<FileEntryModel>();
    private IEnumerable<FileEntryModel> EnumerateAll(string path)
    {
        if (!BasePath.Equals(path, StringComparison.InvariantCultureIgnoreCase) && Directory.GetParent(path) is not null and var parent)
        {
            yield return new FileEntryModel("..", parent.FullName, FileEntryType.History);
        }

        foreach (var directory in Directory.EnumerateDirectories(path))
        {
            yield return new FileEntryModel(Path.GetFileName(directory), directory, FileEntryType.Folder);
        }
        foreach (var file in Directory.EnumerateFiles(path))
        {
            yield return new FileEntryModel(Path.GetFileName(file), file, FileEntryType.Document);
        }
    }
    public ICommand Open { get; }
    public ICommand ShowInfo { get; }

    private readonly Action<string> _fileSelected;
    public ContentViewModel(Action<string> onFileSelected)
    {
        _fileSelected = onFileSelected;
        CurrentFolder = BasePath = @"F:\Git\Titan\samples\Titan.Sandbox\assets";
        FileEntries.Load(EnumerateAll(BasePath));
        Open = ReactiveCommand.Create<FileEntryModel>(fileEntry =>
        {
            if (fileEntry.Type is FileEntryType.Folder or FileEntryType.History)
            {
                CurrentFolder = fileEntry.FullPath;
                FileEntries.Load(EnumerateAll(fileEntry.FullPath));
            }

            _fileSelected(fileEntry.FullPath);
        });

        ShowInfo = ReactiveCommand.Create<FileEntryModel>(fileEntry =>
        {
            Info = fileEntry.FullPath;
            _fileSelected(fileEntry.FullPath);
        });
    }
}
