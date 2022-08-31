using System;
using System.Diagnostics;
using Titan.Core.Logging;
using Titan.FileSystem;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

internal unsafe struct AssetFileAccessor
{
    private ManifestToFile* _files;
    private int _count;

    private PlatformAllocator* _allocator;
    public static bool Create(PlatformAllocator* allocator, FileSystemApi* fileSystem, AssetsConfiguration[] configs, out AssetFileAccessor accessor)
    {
        accessor = default;
        var files = allocator->Allocate<ManifestToFile>(configs.Length);
        if (files == null)
        {
            Logger.Error<AssetFileAccessor>($"Failed to allocate a block for {nameof(ManifestToFile)} with size {sizeof(ManifestToFile) * configs.Length} bytes");
            return false;
        }

        for (var i = 0; i < configs.Length; ++i)
        {
            ref var fileRef = ref files[i];
            fileRef.Id = configs[i].Id;
            if (!AssetFile.Open(configs[i].TitanPakFile, fileSystem, out fileRef.File))
            {
                Logger.Error<AssetFileAccessor>($"Failed to open file {configs[i].TitanPakFile}");
                goto Error;
            }
            Logger.Trace<AssetFileAccessor>($"Opened file {configs[i].TitanPakFile} with size {files[i].File.GetLength()} bytes");
        }
        accessor = new AssetFileAccessor
        {
            _allocator = allocator,
            _files = files,
            _count = configs.Length
        };
        return true;
Error:

//NOTE(Jens): if there's an error, close all open file handles.
        for (var i = 0; i < configs.Length; ++i)
        {
            files[i].File.Close();
        }
        return false;
    }


    public int Read(Span<byte> buffer, in AssetDescriptor descriptor)
    {
        Debug.Assert(buffer.Length >= (int)descriptor.Reference.Size, $"Buffer size is {buffer.Length} bytes, minimum required is {descriptor.Reference.Size} bytes.");

        var file = GetFileForManifest(descriptor.ManifestId);
        Debug.Assert(file != null, $"TitanPak file with ID {descriptor.ManifestId} was not found.");

        return file->Read(buffer[..(int)descriptor.Reference.Size], descriptor.Reference.Offset);
    }

    private AssetFile* GetFileForManifest(uint manifestId)
    {
        for (var i = 0; i < _count; ++i)
        {
            if (_files[i].Id == manifestId)
            {
                return &_files[i].File;
            }
        }
        return null;
    }


    public void Release()
    {
        for (var i = 0; i < _count; ++i)
        {
            _files[i].File.Close();
        }
        _allocator->Free(_files);
        _files = null;
        _count = 0;
    }

    private struct ManifestToFile
    {
        public uint Id;
        public AssetFile File;
    }
}
