using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Memory.Platform;

namespace Titan.Memory;

public static class PlatformAllocatorHelper
{
    /// <summary>
    /// This will create a Heap Allocated PlatformAllocator. This is the only place the NativeMemory class will be used within the engine.
    /// </summary>
    /// <returns>Pointer to the allocator</returns>
    public static unsafe PlatformAllocator* GetPlatformAllocator() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
        {
            true => Win32PlatformAllocator.Instance,
            false or _ => PosixPlatformAllocator.Instance
        };
}


public unsafe struct PlatformAllocator
{
    public readonly uint PageSize;
    private delegate*<void*, uint, void*> _reserve;
    private delegate*<void*, uint, uint, void> _commit;
    private delegate*<void*, uint, uint, void> _decommit;
    private delegate*<void*, uint, void> _release;
    public PlatformAllocator(uint pageSize)
    {
        PageSize = pageSize;
        _reserve = null;
        _commit = null;
        _decommit = null;
        _release = null;
    }

    public static PlatformAllocator Create<T>() where T : IPlatformAllocator =>
        new(T.PageSize)
        {
            _reserve = &T.Reserve,
            _commit = &T.Commit,
            _decommit = &T.Decommit,
            _release = &T.Release
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Reserve(void* startAddress, uint pages) => _reserve(startAddress, pages);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Commit(void* startAddress, uint pages, uint pageOffset = 0) => _commit(startAddress, pages, pageOffset);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Decommit(void* startAddress, uint pages, uint pageOffset = 0) => _decommit(startAddress, pages, pageOffset);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Release(void* startAddress, uint pages) => _release(startAddress, pages);
}
