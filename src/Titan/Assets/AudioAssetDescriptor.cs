using System.Runtime.InteropServices;

namespace Titan.Assets;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct AudioAssetDescriptor
{
    public ushort Channels;
    public ushort BitsPerSample;
    public uint SamplesPerSecond;
}
