using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows;

[DebuggerDisplay("{ToString()}")]
[StructLayout(LayoutKind.Sequential, Size = sizeof(uint))]
public struct DWORD
{
    public uint Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(DWORD dword) => dword.Value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator DWORD(uint dword) => new() { Value = dword };
    public override string ToString() => Value.ToString();
}
