namespace Titan.Tools.Editor.Core;
internal interface IFileSystem
{
    Task<string> ReadString(string path);
    Task<byte[]> ReadBytes(string path);
    IEnumerable<string> EnumerateFolders(string path, string? searchPattern, SearchOption options);
    IEnumerable<string> EnumerateFiles(string path, string? searchPattern, SearchOption options);
    void CreateFolder(string path);
    void CopyFile(string source, string destination, bool overwrite = false);
    Task WriteText(string path, string contents);
    void SetHidden(string path);
    bool Exists(string path);
}
