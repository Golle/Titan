using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows;

[DebuggerDisplay("{ToString()}")]
[StructLayout(LayoutKind.Sequential, Size = sizeof(ushort))]
public struct WORD
{
    private ushort Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ushort(WORD word) => word.Value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator WORD(ushort word) => new() { Value = word };
    public override string ToString() => Value.ToString();
}
