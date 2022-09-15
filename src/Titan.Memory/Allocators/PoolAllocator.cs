using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Titan.Memory.Allocators;

public unsafe interface IPoolAllocator<T> where T : unmanaged
{
    static abstract void* CreateAndInit(MemoryManager* memoryManager, uint maxCount);
    static abstract T* Alloc(void* context, bool initialize);
    static abstract void Free(void* context, T* ptr);
    static abstract void Release(void* context);
}

public unsafe struct PoolAllocator<T> where T : unmanaged
{
    private void* _context;
    private delegate*<void*, bool, T*> _alloc;
    private delegate*<void*, T*, void> _free;
    private delegate*<void*, void> _release;

    public static bool Create<TAllocator>(MemoryManager* memoryManager, uint desiredMaxCount, out PoolAllocator<T> allocator) where TAllocator : unmanaged, IPoolAllocator<T>
    {
        allocator = default;
        Debug.Assert(desiredMaxCount > 0);
        var context = TAllocator.CreateAndInit(memoryManager, desiredMaxCount);
        if (context == null)
        {
            return false;
        }
        allocator = new()
        {
            _context = context,
            _alloc = &TAllocator.Alloc,
            _free = &TAllocator.Free,
            _release = &TAllocator.Release
        };
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Alloc(bool initialize = false)
    {
        Debug.Assert(_context != null);
        return _alloc(_context, initialize);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Free(T* ptr)
    {
        Debug.Assert(_context != null);
        _free(_context, ptr);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Release()
    {
        _release(_context);
        this = default;
    }
}
