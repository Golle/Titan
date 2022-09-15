using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators;

internal unsafe struct DynamicPoolAllocator<T> : IPoolAllocator<T> where T : unmanaged
{
    private static readonly uint AlignedTypeSize = MemoryUtils.AlignToUpper(sizeof(T));

    private MemoryManager* _memoryManager;
    private VirtualMemoryBlock _memory;
    private uint _offsetCount;
    private Header* _freeList;

    public static void* CreateAndInit(MemoryManager* memoryManager, uint maxCount)
    {
        var totalSize = AlignedTypeSize * maxCount;
        if (!memoryManager->TryReserveVirtualMemoryBlock(totalSize, out var memoryBlock))
        {
            Logger.Error<DynamicPoolAllocator<T>>($"Failed to reserve {totalSize} bytes of memory.");
            return null;
        }
        var allocator = memoryManager->Alloc<DynamicPoolAllocator<T>>();
        allocator->_memory = memoryBlock;
        allocator->_offsetCount = 0;
        allocator->_freeList = null;
        allocator->_memoryManager = memoryManager;
        return allocator;
    }

    public static T* Alloc(void* context, bool initialize)
    {
        var instance = (DynamicPoolAllocator<T>*)context;
        Debug.Assert(instance != null);

        T* mem;
        if (instance->_freeList != null)
        {
            mem = (T*)instance->_freeList;
            instance->_freeList = instance->_freeList->Previous;
        }
        else
        {
            mem = GetNext(instance);
        }

        if (mem == null)
        {
            Logger.Error<DynamicPoolAllocator<T>>($"Pool limit of {instance->_memory.MaxSize} bytes has been reached.");
        }
        else if (initialize)
        {
            *mem = default;
        }
        return mem;
    }

    private static T* GetNext(DynamicPoolAllocator<T>* instance)
    {
        var nextItemOffset = instance->_offsetCount * AlignedTypeSize;
        // check if the next item will fit
        if (nextItemOffset + AlignedTypeSize > instance->_memory.Size)
        {
            if (!Expand(instance))
            {
                Logger.Error<DynamicPoolAllocator<T>>("Failed to expand the pool");
                return null;
            }
        }
        instance->_offsetCount++;
        return (T*)((byte*)instance->_memory.Mem + nextItemOffset);
    }

    public static void Free(void* context, T* ptr)
    {
        //NOTE(Jens): implement some way to prevent the same item beeing freed several times. that will corrupt the free list.
        var instance = (DynamicPoolAllocator<T>*)context;
        Debug.Assert(instance != null);

        //NOTE(Jens): add a check that the ptr freed is within this block of memory

        var header = (Header*)ptr;
        header->Previous = instance->_freeList;
        instance->_freeList = header;
    }

    public static void Release(void* context)
    {
        var instance = (DynamicPoolAllocator<T>*)context;
        Debug.Assert(instance != null);

        instance->_memory.Release();
        instance->_memoryManager->Free(context);
    }

    private static bool Expand(DynamicPoolAllocator<T>* instance)
    {
        //NOTE(Jens): multiply current size with 2, if this is the first call it will use the aligned type size.
        //NOTE(Jens): virtual memory will always be page aligned, so for a struct smaller than 4kb it will still allocate an entire block
        var newSize = Math.Max(instance->_memory.Size * 2, AlignedTypeSize);
        newSize = Math.Min(newSize, instance->_memory.MaxSize);
        if (!instance->_memory.Resize(newSize))
        {
            Logger.Error<DynamicPoolAllocator<T>>($"Failed to resize the virtual memory block with {newSize} bytes.");
            return false;
        }
        return true;
    }

    private struct Header
    {
        public Header* Previous;
    }
}
