using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Explicit)]
public struct LARGE_INTEGER
{
    [FieldOffset(0)]
    public DWORD LowPart;
    [FieldOffset(sizeof(int))]
    public int HighPart;
    [FieldOffset(0)]
    public ulong QuadPart;
}
