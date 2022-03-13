using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct HidPRange
{
    public ushort UsageMin, UsageMax;
    public ushort StringMin, StringMax;
    public ushort DesignatorMin, DesignatorMax;
    public ushort DataIndexMin, DataIndexMax;
}
