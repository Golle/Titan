using System.Diagnostics;

namespace Titan.Memory.Allocators;

internal unsafe struct DynamicPoolAllocator<T> : IPoolAllocator<T> where T : unmanaged
{
    private MemoryManager* _memoryManager;
    public static void* CreateAndInit(MemoryManager* memoryManager, uint maxCount)
    {
        throw new NotImplementedException();
    }

    public static T* Alloc(void* context, bool initialize)
    {
        var instance = (DynamicPoolAllocator<T>*)context;
        Debug.Assert(instance != null);

        return null;
    }

    public static void Free(void* context, T* ptr)
    {
        var instance = (DynamicPoolAllocator<T>*)context;
        Debug.Assert(instance != null);
        
        throw new NotImplementedException();
    }

    public static void Release(void* context)
    {
        var instance = (DynamicPoolAllocator<T>*)context;
        Debug.Assert(instance != null);


    }
}
