using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32;

public unsafe struct OVERLAPPED
{
    public ulong* Internal;
    public ulong* InternalHigh;
    private OVERLAPPED_UNION _union;
    [UnscopedRef]
    public ref uint Offset => ref _union.Offset;
    [UnscopedRef]
    public ref uint OffsetHigh => ref _union.OffsetHigh;
    [UnscopedRef]
    public ref void* Pointer => ref _union.Pointer;

    public HANDLE hEvent;

    [StructLayout(LayoutKind.Explicit)]
    private struct OVERLAPPED_UNION
    {
        [FieldOffset(0)]
        public uint Offset;
        [FieldOffset(sizeof(uint))]
        public uint OffsetHigh;
        [FieldOffset(0)]
        public void* Pointer;
    }
}
