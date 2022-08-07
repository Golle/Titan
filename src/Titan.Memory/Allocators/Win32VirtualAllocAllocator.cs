using System;
using System.Diagnostics;
using Titan.Windows.Win32;

namespace Titan.Memory.Allocators;

public unsafe struct Win32VirtualAllocAllocator : IAllocator
{
    public static readonly nuint PageSize = (nuint)Environment.SystemPageSize;
    public static void* Allocate(nuint size)
    {
        //NOTE(Jens): VirtualAlloc will always reserve 64kb address space even for small allocations. We might want to do something that handles that, and "reuse" already reserved space. (MEM_COMMIT will allocate in physical memory, Reserve wont)
        var mem = Kernel32.VirtualAlloc(null, size, AllocationType.MEM_RESERVE | AllocationType.MEM_COMMIT, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(mem != null, $"Failed to allocate {size} bytes of memory.");
        return mem;
    }

    public static void Free(void* ptr)
    {
        var result = Kernel32.VirtualFree(ptr, 0, AllocationType.MEM_RELEASE);
        Debug.Assert(result, "Failed to release memory");
    }
}