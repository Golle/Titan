using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Platform.Posix;

namespace Titan.Core.Memory.Platform;

internal unsafe struct PosixPlatformAllocator : IPlatformAllocator
{
    public static uint PageSize { get; } = (uint)Libc.getpagesize();

    public static readonly PlatformAllocator* Instance;

    static PosixPlatformAllocator()
    {
        Instance = (PlatformAllocator*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(PosixPlatformAllocator), sizeof(PlatformAllocator));
        *Instance = PlatformAllocator.Create<PosixPlatformAllocator>();
    }

    public static void* Reserve(void* startAddress, uint pages)
    {
        var mem = Libc.mmap(startAddress, pages * PageSize, PageProtection.PROT_NONE, PageFlags.MAP_ANON | PageFlags.MAP_SHARED, -1, 0);
        Debug.Assert(mem != null, "Failed to map memory.");
        return mem;
    }

    public static void Commit(void* startAddress, uint pages, uint pageOffset = 0)
    {
        var offset = pageOffset * PageSize;
        var result = Libc.mprotect((byte*)startAddress + offset, pages * PageSize, PageProtection.PROT_READ | PageProtection.PROT_WRITE);
        Debug.Assert(result == 0);
    }

    public static void Decommit(void* startAddress, uint pages, uint pageOffset = 0)
    {
        var offset = pageOffset * PageSize;
        var result = Libc.mprotect((byte*)startAddress + offset, pages * PageSize, PageProtection.PROT_NONE);
        Debug.Assert(result == 0);
    }

    public static void Release(void* startAddress, uint pages)
    {
        var result = Libc.munmap(startAddress, pages * PageSize);
        Debug.Assert(result == 0);
    }
}
