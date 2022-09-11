using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32
{
    [SkipLocalsInit]
    [StructLayout(LayoutKind.Sequential)]
    public struct HMODULE
    {
        public nint Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint(HMODULE module) => module.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HMODULE(nint module) => new HMODULE {Value = module};
    }
}
