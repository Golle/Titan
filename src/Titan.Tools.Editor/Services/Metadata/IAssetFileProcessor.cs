namespace Titan.Tools.Editor.Services.Metadata;

internal record AssetFileInfo(string Name, string FullPath, bool IsDirectory, string? FileExtension, DateTime Changed, long Size);
internal record AssetFileCreatedInfo(string Name, string FullPath, bool IsDirectory, string? FileExtension, DateTime Created);
internal record AssetFileRenameInfo(string PreviousName, string NewName, string PreviousFullPath, string NewFullPath, string? PreviousFileExtension, string? NewFileExtension, bool IsDirectory);
internal record AssetFileDeletedInfo(string Name, string FullPath, string? FileExtension);
internal interface IAssetFileProcessor
{
    /// <summary>
    ///  This function will be called on Created and Changed events. 
    /// </summary>
    /// <param name="info">All info about the file/directory</param>
    Task OnFileChanged(AssetFileInfo info);
    Task OnFileCreated(AssetFileCreatedInfo info);
    Task OnFileRename(AssetFileRenameInfo info);
    Task OnFileDeleted(AssetFileDeletedInfo info);
}
