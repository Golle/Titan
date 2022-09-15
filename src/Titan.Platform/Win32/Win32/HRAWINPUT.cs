using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct HRAWINPUT
{
    public nuint Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nuint(HRAWINPUT rawInput) => rawInput.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator HRAWINPUT(nuint handle) => new() { Value = handle };
}
