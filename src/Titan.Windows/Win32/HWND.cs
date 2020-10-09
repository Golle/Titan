using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential)]
    public struct HWND
    {
        public nint Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint(HWND hwnd) => hwnd.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HWND(nint hwnd) => new HWND{Value = hwnd};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in HWND lh, in HWND rh) => lh.Value == rh.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in HWND lh, in HWND rh) => lh.Value != rh.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is HWND other && other.Value == Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();
    }
}
