using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Controls;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class FileEntryViewModel : ViewModelBase
{
    private bool _selected;
    public string FileName { get; }
    public string FullPath { get; }
    public FileEntryType Type { get; }
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
    public ReactiveCommand<FileEntryViewModel, Unit> Select { get; }

    public ContentViewModel()
    {
        Open = ReactiveCommand.CreateFromTask<FileEntryViewModel>(async fileEntry =>
        {
            if (fileEntry.Type is FileEntryType.Folder or FileEntryType.History)
            {
                CurrentFolder = fileEntry.FullPath;
                FileEntries.Load(EnumerateAll(fileEntry.FullPath));
            }

        });

        Select = ReactiveCommand.CreateFromTask<FileEntryViewModel>(async fileEntry =>
        {
            SelectedFile = FileInfoViewModel.Create(fileEntry.FullPath);
            foreach (var fileEntryViewModel in FileEntries)
            {
                fileEntryViewModel.Selected = false;
            }
            fileEntry.Selected = true;
        });
    }


    public void Load(string path)
    {
        FileEntries.Load(EnumerateAll(path));
    }
    private IEnumerable<FileEntryViewModel> EnumerateAll(string path)
    {
        if (!BasePath.Equals(path, StringComparison.InvariantCultureIgnoreCase) && Directory.GetParent(path) is not null and var parent)
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
}
