using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators;

public unsafe struct DynamicLinearAllocator : ILinearAllocator
{
    private MemoryManager* _memoryManager;
    private VirtualMemoryBlock _memory;
    private uint _offset;
    public static void* CreateAndInit(MemoryManager* memoryManager, uint size)
    {
        if (!memoryManager->TryReserveVirtualMemoryBlock(size, out var block))
        {
            Logger.Error<DynamicLinearAllocator>($"Failed to reserve {size} bytes.");
            return null;
        }
        var instance = memoryManager->Alloc<DynamicLinearAllocator>();
        instance->_memoryManager = memoryManager;
        instance->_memory = block;
        return instance;
    }

    public static void* Allocate(void* context, uint size, bool initialize)
    {
        var instance = (DynamicLinearAllocator*)context;

        while (instance->_offset + size > instance->_memory.Size)
        {
            Expand(instance, size);
        }
        var mem = (byte*)instance->_memory.Mem + instance->_offset;
        instance->_offset += size;
        if (initialize)
        {
            MemoryUtils.Init(mem, size);
        }
        return mem;
    }


    public static void Reset(void* context)
    {
        var instance = (DynamicLinearAllocator*)context;
        instance->_offset = 0;
    }

    public static void Release(void* context)
    {
        var instance = (DynamicLinearAllocator*)context;
        instance->_memory.Release();
        instance->_memoryManager->Free(instance);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void Expand(DynamicLinearAllocator* instance, nuint size)
    {
        // TOOD: add a spin lock
        var currentSize = instance->_memory.Size;
        if (currentSize == 0)
        {
            instance->_memory.Resize(size);
        }
        else
        {
            //NOTE(Jens): double the current size or just increase with whatever size was requested?
            //_memory.Resize(_memory.Size + size); // Increase with the size requested (will be page aligned)
            var newSize = (nuint)Math.Max(instance->_memory.Size * 2u, size); // increase with double size.
            instance->_memory.Resize(newSize);
        }
    }
}
