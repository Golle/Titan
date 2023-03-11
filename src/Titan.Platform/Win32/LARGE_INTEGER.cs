using System.Runtime.InteropServices;

namespace Titan.Platform.Win32;

[StructLayout(LayoutKind.Explicit)]
public struct LARGE_INTEGER
{
    [FieldOffset(0)]
    public uint LowPart;
    [FieldOffset(sizeof(int))]
    public int HighPart;
    [FieldOffset(0)]
    public ulong QuadPart;
}
