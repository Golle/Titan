using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{

    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HRESULT
    {
        public int Value;
        public override string ToString() => $"0x{Value:X}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(HRESULT hresult) => hresult.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static implicit operator HRESULT(int hresult) => new HRESULT {Value = hresult};
    }
}
