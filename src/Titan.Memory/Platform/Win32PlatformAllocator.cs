using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Platform.Win32.Win32;

namespace Titan.Memory.Platform;

internal unsafe struct Win32PlatformAllocator : IPlatformAllocator
{
    public static uint PageSize { get; } = GetPageSize();
    public static void* Reserve(void* startAddress, uint pages)
    {
        Logger.Trace<Win32PlatformAllocator>($"Reserve {pages} pages ({pages * PageSize} bytes) on start address: 0x{(nuint)startAddress}");
        var mem = Kernel32.VirtualAlloc(startAddress, pages * PageSize, AllocationType.MEM_RESERVE, AllocationProtect.PAGE_NOACCESS);
        Debug.Assert(mem != null);
        return mem;
    }

    public static void Commit(void* startAddress, uint pages, uint pageOffset = 0)
    {
        Logger.Trace<Win32PlatformAllocator>($"Commit virtual memory. Pages: {pages}. PageOffset: {pageOffset} on start address: 0x{(nuint)startAddress}");
        var offset = pageOffset * PageSize;
        var result = Kernel32.VirtualAlloc((byte*)startAddress + offset, pages * PageSize, AllocationType.MEM_COMMIT, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(result != null);
    }

    public static void Decommit(void* startAddress, uint pages, uint pageOffset = 0)
    {
        Logger.Trace<Win32PlatformAllocator>($"Decommit virtual memory. Pages: {pages}. PageOffset: {pageOffset} on start address: 0x{(nuint)startAddress}");
        var offset = pageOffset * PageSize;
        var result = Kernel32.VirtualFree((byte*)startAddress + offset, pages * PageSize, AllocationType.MEM_DECOMMIT);
        Debug.Assert(result);
    }

    public static void Release(void* startAddress, uint pages)
    {
        Logger.Trace<Win32PlatformAllocator>($"Releae virtual memory. Pages: {pages}. Address: 0x{(nuint)startAddress}");
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
