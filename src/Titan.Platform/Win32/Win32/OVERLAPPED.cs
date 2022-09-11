using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

public unsafe struct OVERLAPPED
{
    public ulong* Internal;
    public ulong* InternalHigh;
    
    private OVERLAPPED_UNION _union;
    public ref DWORD Offset => ref ((OVERLAPPED_UNION*)Unsafe.AsPointer(ref _union))->Offset;
    public ref DWORD OffsetHigh => ref ((OVERLAPPED_UNION*)Unsafe.AsPointer(ref _union))->OffsetHigh;
    public ref void* Pointer => ref ((OVERLAPPED_UNION*)Unsafe.AsPointer(ref _union))->Pointer;
    public HANDLE hEvent;

    [StructLayout(LayoutKind.Explicit)]
    private struct OVERLAPPED_UNION
    {
        [FieldOffset(0)]
        public DWORD Offset;
        [FieldOffset(sizeof(uint))]
        public DWORD OffsetHigh;
        [FieldOffset(0)]
        public void* Pointer;
    }
}
