using System.Diagnostics;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Memory.Allocators;

namespace Titan.Memory;

public unsafe struct PlatformAllocator : IApi
{
    private Allocator _allocator;
    internal Allocator* UnderlyingAllocator => MemoryUtils.AsPointer(ref _allocator);

    public readonly T* Allocate<T>(int count = 1, bool initialize = true) where T : unmanaged
        => Allocate<T>((uint)count, initialize);

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
            MemoryUtils.Init(memory, size);
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
            Logger.Warning<PlatformAllocator>("The fixed size allocator does not support Free, this means that any code that tries to free the memory will have a memory leak.");
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
