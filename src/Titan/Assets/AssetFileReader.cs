using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Assets;

internal class AssetFileReader : IAssetFileReader 
{
    private TitanArray<AssetFile> _files;
    private IMemoryManager _memoryManager;
    private IFileSystem _fileSystem;

    public bool Init(IMemoryManager memoryManager, IFileSystem fileSystem, AssetsConfiguration[] configs)
    {
        _files = memoryManager.AllocArray<AssetFile>((uint)configs.Length, true);
        if (!_files.IsValid)
        {
            Logger.Error<AssetFileReader>($"Failed to allocate memory for {configs.Length} {nameof(AssetFile)}");
            return false;
        }

        for (var i = 0; i < configs.Length; ++i)
        {
            var config = configs[i];
            var handle = fileSystem.Open(config.TitanPakFile, FileAccess.Read, FileMode.Open);
            if (handle.IsInvalid())
            {
                Logger.Error<AssetFileReader>($"Failed to open file at path {config.TitanPakFile}.");
                goto Error;
            }

            var fileSize = fileSystem.GetLength(handle);
            Logger.Trace<AssetFileReader>($"Opened file: {config.TitanPakFile} Length: {fileSize} bytes.");
            _files[i] = new AssetFile
            {
                Handle = handle,
                ManifestId = config.Id,
                Size = fileSize
            };
        }
        _memoryManager = memoryManager;
        _fileSystem = fileSystem;

        return true;
Error:
        foreach (ref var file in _files.AsSpan())
        {
            if (file.Handle.IsValid())
            {
                fileSystem.Close(ref file.Handle);
            }
        }
        memoryManager.Free(ref _files);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetSizeFromDescriptor(in AssetDescriptor descriptor) 
        => descriptor.GetSize();

    public int Read(Span<byte> buffer, in AssetDescriptor descriptor)
    {
        Debug.Assert(buffer.Length >= (int)descriptor.Reference.Size, $"Buffer size is {buffer.Length} bytes, minimum required is {descriptor.Reference.Size} bytes.");
        ref readonly var reference = ref descriptor.Reference;
        Debug.Assert(reference.Size < int.MaxValue, "Huge files are not supported.");
        var fileHandle = GetFileHandle(descriptor.ManifestId);
        if (fileHandle.IsInvalid())
        {
            Logger.Error<AssetFileReader>($"Trying to read from a file that has not been registered. ManifestId: {descriptor.ManifestId}");
            return -1;
        }
        // create a slice of the span that has the same size as the data we're reading to avoid over reads or read past EOF.
        return _fileSystem!.Read(fileHandle, buffer[..(int)reference.Size], descriptor.Reference.Offset);
    }

    private FileHandle GetFileHandle(uint manifestId)
    {
        foreach (ref readonly var file in _files.AsReadOnlySpan())
        {
            if (file.ManifestId == manifestId)
            {
                return file.Handle;
            }
        }
        return FileHandle.Invalid;
    }

    public void Shutdown()
    {
        if (_memoryManager != null && _fileSystem != null)
        {
            foreach (ref var file in _files.AsSpan())
            {
                _fileSystem.Close(ref file.Handle);
            }
            _memoryManager.Free(ref _files);
        }
        _memoryManager = null;
        _fileSystem = null;
    }

    private struct AssetFile
    {
        public uint ManifestId;
        public FileHandle Handle;
        public long Size;
    }
}
