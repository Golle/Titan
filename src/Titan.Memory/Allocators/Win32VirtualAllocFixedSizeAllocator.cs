using System.Diagnostics;
using System.Threading;
using Titan.Core.Logging;
using Titan.Windows.Win32;

namespace Titan.Memory.Allocators;

public record struct FixedSizeArgs(nuint Size);
public unsafe struct Win32VirtualAllocFixedSizeAllocator : IAllocator<FixedSizeArgs>
{
    private byte* _memory;
    private nuint _size;
    private volatile int _offset;
    public static void* CreateContext(in FixedSizeArgs args)
    {
        //NOTE(Jens): This should be aligned at 64kb since that is what VirtualAlloc does. 
        var size = args.Size;
        var contextSize = (nuint)sizeof(Win32VirtualAllocAllocator);
        var mem = (byte*)Kernel32.VirtualAlloc(null, size + contextSize, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(mem != null, "Failed to allocate memory");

        //TODO: Consider putting the context at the top.
        var context = (Win32VirtualAllocFixedSizeAllocator*)(mem + size);
        context->_memory = mem;
        context->_offset = 0;
        context->_size = size;

        return context;
    }

    public static void ReleaseContext(void* context)
    {
        var memoryContext = (Win32VirtualAllocFixedSizeAllocator*)context;
        Kernel32.VirtualFree(memoryContext->_memory, 0, AllocationType.MEM_RELEASE);
    }

    public static void* Allocate(void* context, nuint size)
    {
        var memoryContext = (Win32VirtualAllocFixedSizeAllocator*)context;
        var offset = Interlocked.Add(ref memoryContext->_offset, (int)size);
        Debug.Assert(offset < (int)memoryContext->_size, "Out of memory");
        return memoryContext->_memory + offset - size;
    }

    public static void Free(void* context, void* ptr)
    {
        Logger.Warning<Win32VirtualAllocFixedSizeAllocator>($"Free is not supported in {nameof(Win32VirtualAllocFixedSizeAllocator)} yet. This means that the code calling this function does have a memory leak.");
        // noop, can't free memory in the fixed context (can be solved with a FreeList)
    }
}
