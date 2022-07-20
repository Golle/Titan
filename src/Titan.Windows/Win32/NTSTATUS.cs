using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[SkipLocalsInit]
[StructLayout(LayoutKind.Sequential)]
public struct NTSTATUS
{
    public int Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(NTSTATUS ntstatus) => ntstatus.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    public static implicit operator NTSTATUS(int ntstatus) => new() { Value = ntstatus };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    public static implicit operator NTSTATUS(uint ntstatus) => new() { Value = (int)ntstatus };
}
