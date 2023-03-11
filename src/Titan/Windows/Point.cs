using System.Runtime.InteropServices;

namespace Titan.Windows;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X, Y;
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
