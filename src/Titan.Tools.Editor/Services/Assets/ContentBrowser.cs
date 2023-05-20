using Titan.Tools.Editor.Core;

namespace Titan.Tools.Editor.Services.Assets;

internal class ContentBrowser : IContentBrowser
{
    private readonly IFileSystem _fileSystem;

    public ContentBrowser(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<IEnumerable<FileEntry>> GetFiles(string path, string? filter = null)
    {
        //NOTE(Jens): this is async and returns a task because we want to read the contents of some files as well.
        var result = await Task.Run(() =>
        {
            List<FileEntry> entries = new();
            foreach (var folder in _fileSystem.EnumerateFolders(path, null, SearchOption.TopDirectoryOnly))
            {
                entries.Add(new FileEntry
                {
                    Name = Path.GetFileName(folder),
                    Path = Path.GetRelativePath(path, folder),
                    Type = FileEntryType.Folder
                });
            }
            foreach (var file in _fileSystem.EnumerateFiles(path, null, SearchOption.TopDirectoryOnly))
            {
                entries.Add(new FileEntry
                {
                    Name = Path.GetFileName(file),
                    Path = Path.GetRelativePath(path, file),
                    Type = FileEntryType.File
                });
            }
            return entries;
        });
        return result;
    }
}
