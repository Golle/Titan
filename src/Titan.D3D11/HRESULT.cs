using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.D3D11
{

    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential)]
    public struct HRESULT
    {
        public int Value;
        public override string ToString() => $"0x{Value:X}";
    }
}
