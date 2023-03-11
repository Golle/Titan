using System.Runtime.InteropServices;
namespace Titan.Platform.Win32.WIC;

[StructLayout(LayoutKind.Sequential)]
public struct WICRect
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
}
