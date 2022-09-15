using System.Diagnostics;
using Titan.Core.Logging;
using Titan.FileSystem;

namespace Titan.Assets.NewAssets;

internal unsafe struct AssetFile
{
    private FileHandle _handle;
    private FileSystemApi* _fileSystem;
    private long _length;

    public static bool Open(ReadOnlySpan<char> path, FileSystemApi* fileSystem, out AssetFile file)
    {
        file = default;
        var handle = fileSystem->Open(path);
        if (!handle.IsValid)
        {
            Logger.Error<AssetFile>($"Failed to open file at path {path}.");
            return false;
        }
        var length = fileSystem->GetLength(handle);
        file = new AssetFile
        {
            _handle = handle,
            _fileSystem = fileSystem,
            _length = length
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

    public int Read(Span<byte> buffer, ulong offset)
    {
        Debug.Assert(_handle.IsValid);
        return _fileSystem->Read(_handle, buffer, offset);
    }

    public long GetLength() => _length;
}
