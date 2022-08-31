using System;
using Titan.Core.Logging;
using Titan.FileSystem;

namespace Titan.Assets.NewAssets;

public unsafe struct AssetFile
{
    private FileHandle _handle;
    private FileSystemApi* _fileSystem;
    public static bool Open(ReadOnlySpan<char> path, FileSystemApi* fileSystem, out AssetFile file)
    {
        file = default;
        var handle = fileSystem->Open(path);
        if (!handle.IsValid)
        {
            Logger.Error<AssetFile>($"Failed to open file at path {path}.");
            return false;
        }
        file = new AssetFile
        {
            _handle = handle,
            _fileSystem = fileSystem
        };
        return true;
    }

    public void Close()
    {
        if (_handle.IsValid)
        {
            _fileSystem->Close(ref _handle);
        }
    }

    public long GetLength() => _fileSystem->GeLength(_handle);
}
