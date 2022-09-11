using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct HidPNotRange
{
    public ushort Usage, Reserved1;
    public ushort StringIndex, Reserved2;
    public ushort DesignatorIndex, Reserved3;
    public ushort DataIndex, Reserved4;
}
