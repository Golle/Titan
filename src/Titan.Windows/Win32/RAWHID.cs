using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct RAWHID
{
    public DWORD dwSizeHid;    // byte size of each report
    public DWORD dwCount;      // number of input packed
    public unsafe fixed byte bRawData[1];
}
