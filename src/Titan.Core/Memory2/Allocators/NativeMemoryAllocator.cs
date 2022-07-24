using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Titan.Core.Memory2.Allocators;

/// <summary>
/// The most basic allocator, uses the NativeMemory class in .NET and works on all platforms.
/// </summary>
public unsafe struct NativeMemoryAllocator : IAllocator
{
    public static void* Allocate(nuint size)
    {
        var mem = NativeMemory.Alloc(size);
        Debug.Assert(mem != null, $"Failed to allocate {size} bytes of memory.");
        return mem;
    }
    public static void Free(void* ptr) => NativeMemory.Free(ptr);
}
