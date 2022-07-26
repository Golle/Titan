using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Memory.Allocators;

namespace Titan.Memory;

public unsafe struct PlatformAllocator : IApi
{
    private Allocator _allocator;

    internal Allocator* UnderlyingAllocator => (Allocator*)Unsafe.AsPointer(ref _allocator);

    //NOTE(Jens): Should we introduce some kind of MemoryBlock in addition to raw pointers?
    public readonly T* Allocate<T>(uint count = 1u, bool initialize = true) where T : unmanaged
    {
        Debug.Assert(count != 0, "Must allocate at least 1 item");
        return (T*)Allocate(count * (nuint)sizeof(T), initialize);
    }

    public readonly void* Allocate(nuint size, bool initialize = true)
    {
        var memory = _allocator.Allocate(size);
        if (initialize)
        {
            // This wont work for allocations greater than 4gb
            Unsafe.InitBlockUnaligned(memory, 0, (uint)size);
            return memory;
        }
        return memory;
    }

    /// <summary>
    /// Release the underlying memory pool. This will only release allocaters that have a context. For example <see cref="Win32VirtualAllocFixedSizeAllocator"/>
    /// </summary>
    internal void Release()
        => _allocator.Release();

    public static PlatformAllocator Create(nuint fixedSizeMemory = 0u) =>
        new()
        {
            _allocator = CreateAllocator(fixedSizeMemory)
        };

    private static Allocator CreateAllocator(nuint fixedSizeMemory)
    {
        if (fixedSizeMemory > 0)
        {
            Logger.Trace<PlatformAllocator>($"Creating a {nameof(Win32VirtualAllocFixedSizeAllocator)} allocator with {fixedSizeMemory} bytes pre-allocated.");
            return Allocator.Create<Win32VirtualAllocFixedSizeAllocator, FixedSizeArgs>(new FixedSizeArgs(fixedSizeMemory));
        }
        // Use VirtualAlloc on windows environment
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Logger.Trace<PlatformAllocator>($"Creating a {nameof(Win32VirtualAllocAllocator)}.");
            return Allocator.Create<Win32VirtualAllocAllocator>();
        }

        // Use the built in NativeMemory on any other platforms
        Logger.Trace<PlatformAllocator>($"Creating a {nameof(NativeMemoryAllocator)}.");
        return Allocator.Create<NativeMemoryAllocator>();
    }

    public readonly void Free(void* ptr) 
        => _allocator.Free(ptr);
}
