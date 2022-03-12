using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct WPARAM
{
    public nuint Value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nuint(WPARAM param) => param.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator WPARAM(nuint param) => new() { Value = param };
}
