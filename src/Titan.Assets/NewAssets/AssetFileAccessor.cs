using System;
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.FileSystem;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

internal unsafe struct AssetFileAccessor
{
    private TitanArray<ManifestToFile> _files;
    private MemoryManager* _memoryManager;

    public bool Init(MemoryManager* memoryManager, FileSystemApi* fileSystem, AssetsConfiguration[] configs)
    {
        Debug.Assert(_files.Length == 0);
        var files = memoryManager->AllocArray<ManifestToFile>((uint)configs.Length);
        if (files.Length == 0)
        {
            Logger.Error<AssetFileAccessor>($"Failed to allocate a block for {nameof(ManifestToFile)} with size {sizeof(ManifestToFile) * configs.Length} bytes");
            return false;
        }
        _memoryManager = memoryManager;
        _files = new TitanArray<ManifestToFile>(files, configs.Length);

        for (var i = 0; i < configs.Length; ++i)
        {
            ref var fileRef = ref _files[i];
            fileRef.Id = configs[i].Id;
            if (!AssetFile.Open(configs[i].TitanPakFile, fileSystem, out fileRef.File))
            {
                Logger.Error<AssetFileAccessor>($"Failed to open file {configs[i].TitanPakFile}");
                goto Error;
            }
            Logger.Trace<AssetFileAccessor>($"Opened file {configs[i].TitanPakFile} with size {_files[i].File.GetLength()} bytes");
        }
        return true;

Error:
        Release();
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
        for (var i = 0; i < _files.Length; ++i)
        {
            var manifestToFile = _files.GetPointer() + i;
            if (manifestToFile->Id == manifestId)
            {
                return &manifestToFile->File;
            }
        }
        return null;
    }

    public void Release()
    {
        foreach (ref var file in _files.AsSpan())
        {
            file.File.Close();
        }
        _memoryManager->Free(_files);
        this = default;
    }

    private struct ManifestToFile
    {
        public uint Id;
        public AssetFile File;
    }
}
