using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators;

internal unsafe struct FixedSizePoolAllocator<T> : IPoolAllocator<T> where T : unmanaged
{
    private MemoryManager* _memoryManager;
    private byte* _mem;
    private Header* _freeList;
    private uint _maxCount;

    public static void* CreateAndInit(MemoryManager* memoryManager, uint maxCount)
    {
        var alignedSize = MemoryUtils.AlignToUpper(sizeof(T));
        var alignedAllocatorSize = MemoryUtils.AlignToUpper(sizeof(FixedSizePoolAllocator<T>));
        var totalSize = alignedAllocatorSize + alignedSize * maxCount;

        var mem = (byte*)memoryManager->Alloc(totalSize, initialize: true);
        if (mem == null)
        {
            Logger.Error<FixedSizePoolAllocator<T>>($"Failed to allocate {totalSize} bytes of memory.");
            return null;
        }
        var allocator = (FixedSizePoolAllocator<T>*)mem;
        allocator->_memoryManager = memoryManager;
        allocator->_mem = mem + alignedAllocatorSize;
        allocator->_maxCount = maxCount;
        InitFreeList(allocator, alignedSize);

        return mem;

        static void InitFreeList(FixedSizePoolAllocator<T>* allocator, uint alignedSize)
        {
            //NOTE(Jens): Initialize the free list in reverse order to put the first element in the array on the last spot (which is accessed first)
            Header* header = null;
            for (var i = (int)allocator->_maxCount - 1; i >= 0; --i)
            {
                var next = (Header*)(allocator->_mem + i * alignedSize);
                next->Previous = header;
                header = next;
            }
            allocator->_freeList = header;
        }
    }

    public static T* Alloc(void* context, bool initialize)
    {
        var instance = (FixedSizePoolAllocator<T>*)context;
        Debug.Assert(instance != null);
        var mem = (T*)instance->_freeList;
        if (mem == null)
        {
            //NOTE(Jens): not sure what we should do here. if this happens all memory in the pool has been allocated.
            return null;
        }
        instance->_freeList = instance->_freeList->Previous;
        if (initialize)
        {
            *mem = default;
        }
        return mem;
    }

    public static void Free(void* context, T* ptr)
    {
        var instance = (FixedSizePoolAllocator<T>*)context;
        Debug.Assert(instance != null);

        var header = (Header*)ptr;
        header->Previous = instance->_freeList;
        instance->_freeList = header;
    }

    public static void Release(void* context)
    {
        var instance = (FixedSizePoolAllocator<T>*)context;
        instance->_memoryManager->Free(context);
    }

    private struct Header
    {
        public Header* Previous;
    }
}
