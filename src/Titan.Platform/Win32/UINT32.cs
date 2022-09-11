using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32;

[DebuggerDisplay("{ToString()}")]
[StructLayout(LayoutKind.Sequential, Size = sizeof(uint))]
public struct UINT32
{
    private uint Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(UINT32 value) => value.Value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UINT32(uint value) => new() { Value = value };
    public override string ToString() => Value.ToString();
}
