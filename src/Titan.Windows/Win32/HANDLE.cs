using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct HANDLE
{
    public nuint Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nuint(HANDLE rawInput) => rawInput.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator HANDLE(nuint handle) => new() { Value = handle };
}
