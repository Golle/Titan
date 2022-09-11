using System.Diagnostics;
using Titan.Platform.Win32.Win32;

namespace Titan.Memory.Platform;

internal unsafe struct Win32PlatformAllocator : IPlatformAllocator
{
    public static uint PageSize { get; } = GetPageSize();
    public static void* Reserve(void* startAddress, uint pages)
    {
        var mem = Kernel32.VirtualAlloc(startAddress, pages * PageSize, AllocationType.MEM_RESERVE, AllocationProtect.PAGE_NOACCESS);
        Debug.Assert(mem != null);
        return mem;
    }

    public static void Commit(void* startAddress, uint pages, uint pageOffset = 0)
    {
        var offset = pageOffset * PageSize;
        var result = Kernel32.VirtualAlloc((byte*)startAddress + offset, pages * PageSize, AllocationType.MEM_COMMIT, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(result != null);
    }

    public static void Decommit(void* startAddress, uint pages, uint pageOffset = 0)
    {
        var offset = pageOffset * PageSize;
        var result = Kernel32.VirtualFree((byte*)startAddress + offset, pages * PageSize, AllocationType.MEM_DECOMMIT);
        Debug.Assert(result);
    }

    public static void Release(void* startAddress, uint pages)
    {
        var result = Kernel32.VirtualFree(startAddress, 0 /*pages * PageSize*/, AllocationType.MEM_RELEASE);
        Debug.Assert(result);
    }

    private static uint GetPageSize()
    {
        SYSTEM_INFO info;
        Kernel32.GetSystemInfo(&info);
        return info.dwPageSize;
    }
}
