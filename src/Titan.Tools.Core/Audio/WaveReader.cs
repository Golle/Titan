using System.Runtime.InteropServices;
using Titan.Core.Memory;
using Titan.Platform.Win32.XAudio2;

namespace Titan.Tools.Core.Audio;

[Flags]
file enum WaveChunkTypes : uint
{
    RIFF = ('R') | ('I' << 8) | ('F' << 16) | ('F' << 24),
    DATA = ('d') | ('a' << 8) | ('t' << 16) | ('a' << 24),
    FMT = ('f') | ('m' << 8) | ('t' << 16) | (' ' << 24)
}

[Flags]
file enum WaveFileType : uint
{
    WAVE = ('W') | ('A' << 8) | ('V' << 16) | ('E' << 24),
    XWMA = ('X') | ('W' << 8) | ('M' << 16) | ('A' << 24),
    DPDS = ('d') | ('p' << 8) | ('d' << 16) | ('s' << 24),
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
file struct WaveChunkHeader
{
    public WaveChunkTypes Type;
    public uint Size;
}

public record ReadWaveResult(bool Success, string? Error, WaveSound? Sound)
{
    public static ReadWaveResult Ok(WaveSound sound) => new(true, null, sound);
    public static ReadWaveResult Fail(string error) => new(true, error, null);
}

public class WaveSound
{
    public required WAVEFORMATEX Format { get; init; } // should have our own format.
    public required byte[] Data { get; init; }
}

public static class WaveReader
{
    public static unsafe ReadWaveResult Read(string path)
    {
        if (!File.Exists(path))
        {
            return ReadWaveResult.Fail($"Failed to find a file at path {path}");
        }

        WAVEFORMATEX format = default;
        ReadOnlySpan<byte> data = default;
        var bytes = File.ReadAllBytes(path);

        fixed (byte* pBytes = bytes)
        {
            var size = bytes.Length;

            var current = pBytes;
            var lastByte = current + size;

            while (current < lastByte)
            {
                var header = (WaveChunkHeader*)current;
                current += sizeof(WaveChunkHeader);

                switch (header->Type)
                {
                    case WaveChunkTypes.RIFF:
                        var fileType = *(WaveFileType*)current;
                        if (fileType is not WaveFileType.WAVE or WaveFileType.XWMA)
                        {
                            return ReadWaveResult.Fail($"Wave file type {fileType} is not supported.");
                        }
                        current += sizeof(WaveFileType);
                        break;
                    case WaveChunkTypes.DATA:
                        data = new ReadOnlySpan<byte>(current, (int)header->Size);
                        current += header->Size;
                        break;
                    case WaveChunkTypes.FMT:
                        MemoryUtils.Copy(&format, current, header->Size);
                        format.cbSize = (ushort)sizeof(WAVEFORMATEX); // Should this be header->Size or sizeof(WAVEFORMATEX) ?
                        current += header->Size;
                        break;
                    default:
                        current += header->Size;
                        break;
                }

            }
        }

        if (data.IsEmpty)
        {
            return ReadWaveResult.Fail("No sound data was found");
        }
        return ReadWaveResult.Ok(new WaveSound
        {
            Data = data.ToArray(),
            Format = format
        });
    }

}
