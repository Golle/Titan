using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Sound.Loaders;

[Flags]
internal enum WaveChunkTypes : uint
{
    RIFF = ('R') | ('I' << 8) | ('F' << 16) | ('F' << 24),
    DATA = ('d') | ('a' << 8) | ('t' << 16) | ('a' << 24),
    FMT = ('f') | ('m' << 8) | ('t' << 16) | (' ' << 24)
}

[Flags]
internal enum WaveFileType : uint
{
    WAVE = ('W') | ('A' << 8) | ('V' << 16) | ('E' << 24),
    XWMA = ('X') | ('W' << 8) | ('M' << 16) | ('A' << 24),
    DPDS = ('d') | ('p' << 8) | ('d' << 16) | ('s' << 24),
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct WaveChunkHeader
{
    public WaveChunkTypes Type;
    public uint Size;
}

public unsafe class WaveLoader : IAssetLoader
{
    private readonly SoundManager _manager;

    public WaveLoader(SoundManager manager)
    {
        _manager = manager;
    }

    public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
    {
        Debug.Assert(buffers.Length == 1, "Only a single file can be loaded.");

        var bytes = buffers[0];
        var pBytes = bytes.AsPointer();
        var lastBytes = pBytes + bytes.Size;
        ReadOnlySpan<byte> data = default;
        WAVEFORMATEX format = default;
        Debug.Assert(pBytes != null);

        while (pBytes < lastBytes)
        {
            var header = (WaveChunkHeader*)pBytes;
            pBytes += sizeof(WaveChunkHeader);
            switch (header->Type)
            {
                case WaveChunkTypes.RIFF:
                    var fileType = *(WaveFileType*)pBytes;
                    if (fileType is not WaveFileType.WAVE or WaveFileType.XWMA)
                    {
                        Logger.Error<WaveLoader>("Wave file type is not supported.");
                        throw new NotSupportedException($"Wave file type {fileType} is not supported.");
                    }
                    pBytes += sizeof(WaveFileType);
                    break;
                case WaveChunkTypes.DATA:
                    data = new ReadOnlySpan<byte>(pBytes, (int)header->Size); // NOTE(jens): this data will be destroyed at the end of the function
                    pBytes += header->Size;
                    break;
                case WaveChunkTypes.FMT:
                    MemoryUtils.Copy(&format, pBytes, header->Size);
                    format.cbSize = (WORD)sizeof(WAVEFORMATEX); // Should this be header->Size or sizeof(WAVEFORMATEX) ?
                    pBytes += header->Size;
                    break;
                default:
                    pBytes += header->Size;
                    break;
            }
        }

        return _manager.Create(new SoundClipCreation
        {
            Data = data,
            Format = format
        });
    }

    public void OnRelease(int handle)
    {
        _manager.Release(handle);
    }

    public void Dispose()
    {

    }
}
