namespace Titan.Tools.Editor.Services.Assets;

public enum FileEntryType
{
    Folder,
    ParentFolder,
    File
}

public record FileEntry
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required FileEntryType Type { get; init; }
}

public interface IContentBrowser
{
    Task<IEnumerable<FileEntry>> GetFiles(string path, string? filter = null);
}
