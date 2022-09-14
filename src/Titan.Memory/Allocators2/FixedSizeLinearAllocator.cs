using System.Diagnostics;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators2;

internal unsafe struct FixedSizeLinearAllocator : ILinearAllocator
{
    private MemoryManager* memoryManager;
    private byte* _mem;
    private uint _size;
    private int _offset; //NOTE(Jens): only supports up to 2gb

    public static void* CreateAndInit(MemoryManager* memoryManager, uint size)
    {
        Debug.Assert(size <= int.MaxValue);

        //NOTE(Jens): keep the instance of the allocator and memory in the same block.
        var instanceAlignedSize = MemoryUtils.AlignToUpper((uint)sizeof(FixedSizeLinearAllocator));
        var totalSize = instanceAlignedSize + size;
        var mem = (byte*)memoryManager->Alloc(totalSize, initialize: true);

        var instance = (FixedSizeLinearAllocator*)mem;
        instance->_mem = mem + instanceAlignedSize;
        instance->_offset = 0;
        instance->_size = size;
        instance->memoryManager = memoryManager;

        return mem;
    }

    public static void* Allocate(void* context, uint size, bool initialize)
    {
        var instance = (FixedSizeLinearAllocator*)context;
        Debug.Assert(instance->_mem != null);
        Debug.Assert(instance->_offset + size < instance->_size);

        var mem = instance->_mem + instance->_offset;
        instance->_offset += (int)size;
        return mem;
    }

    public static void Reset(void* context)
    {
        var instance = (FixedSizeLinearAllocator*)context;
        instance->_offset = 0;
    }

    public static void Release(void* context)
    {
        var instance = (FixedSizeLinearAllocator*)context;
        instance->memoryManager->Free(context);
    }
}
