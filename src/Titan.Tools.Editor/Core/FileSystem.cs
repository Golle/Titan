namespace Titan.Tools.Editor.Core;

internal class FileSystem : IFileSystem
{
    public FileStream OpenWrite(string path) 
        => File.OpenWrite(path);

    public FileStream OpenRead(string path) 
        => File.OpenRead(path);

    public Task<string> ReadString(string path)
        => File.ReadAllTextAsync(path);

    public Task<byte[]> ReadBytes(string path)
        => File.ReadAllBytesAsync(path);
    public IEnumerable<string> EnumerateFolders(string path, string? searchPattern, SearchOption options)
        => Directory.EnumerateDirectories(path, searchPattern ?? string.Empty, options);
    public IEnumerable<string> EnumerateFiles(string path, string? searchPattern, SearchOption options)
        => Directory.EnumerateFiles(path, searchPattern ?? string.Empty, options);

    public void CreateFolder(string path)
        => Directory.CreateDirectory(path);

    public void CopyFile(string source, string destination, bool overwrite = false)
        => File.Copy(source, destination, overwrite);

    public Task WriteText(string path, string contents)
        => File.WriteAllTextAsync(path, contents);

    public void SetHidden(string path)
    {
        if (Directory.Exists(path))
        {
            new DirectoryInfo(path).Attributes |= FileAttributes.Hidden;
        }
        else if (File.Exists(path))
        {
            new FileInfo(path).Attributes |= FileAttributes.Hidden;
        }
    }

    public bool Exists(string path) 
        => File.Exists(path);
}
