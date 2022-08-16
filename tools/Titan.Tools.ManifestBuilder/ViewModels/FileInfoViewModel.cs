using System;
using System.IO;
using ReactiveUI;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class FileInfoViewModel : ViewModelBase
{
    public void FileSelected(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            FileName = "n/a";
            return;
        }

        var fileInfo = new FileInfo(path);
        Size = (fileInfo.Attributes & FileAttributes.Directory) == 0 ? fileInfo.Length : null;
        FileName = fileInfo.Name;
        FileCreated = fileInfo.CreationTime.ToString("yy-MM-dd HH:mm:ss");
        FileChanged = fileInfo.LastWriteTime.ToString("yy-MM-dd HH:mm:ss");
    }

    private long? _size;
    public long? Size
    {
        get => _size;
        private set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    private string? _fileName;
    public string? FileName
    {
        get => _fileName;
        private set => this.RaiseAndSetIfChanged(ref _fileName, value);
    }

    private string _fileChanged = string.Empty;
    public string FileChanged
    {
        get => _fileChanged;
        private set => this.RaiseAndSetIfChanged(ref _fileChanged, value);
    }
    private string _fileCreated = string.Empty;
    public string FileCreated
    {
        get => _fileCreated;
        private set => this.RaiseAndSetIfChanged(ref _fileCreated, value);
    }

}
