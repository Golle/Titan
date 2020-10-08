using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.D3D11
{
    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential)]
    public struct HWND
    {
        public nint Value;

        public static implicit operator nint(HWND hwnd) => hwnd.Value;
        public static implicit operator HWND(nint hwnd) => new HWND{Value = hwnd};
    }
}
