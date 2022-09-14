using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators2;
public unsafe interface ILinearAllocator
{
    static abstract void* CreateAndInit(MemoryManager* memoryManager, uint size);
    static abstract void* Allocate(void* context, uint size, bool initialize);
    static abstract void Reset(void* context);
    static abstract void Release(void* context);
}

public unsafe struct LinearAllocator
{
    private void* _context;
    private delegate*<void*, uint, bool, void*> _alloc;
    private delegate*<void*, void> _reset;
    private delegate*<void*, void> _release;

    public static bool Create<T>(MemoryManager* memoryManager, uint size, out LinearAllocator allocator) where T : unmanaged, ILinearAllocator
    {
        allocator = default;
        Debug.Assert(size > 0);
        var context = T.CreateAndInit(memoryManager, size);
        if (context == null)
        {
            return false;
        }

        allocator = new LinearAllocator
        {
            _context = context,
            _alloc = &T.Allocate,
            _release = &T.Release,
            _reset = &T.Reset
        };
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(int size, bool initialize = false)
    {
        Debug.Assert(size >= 0);
        return Alloc((uint)size, initialize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(uint size, bool initialize = false)
    {
        Debug.Assert(_context != null);
        var mem = _alloc(_context, size, initialize);
        return mem;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Alloc<T>(bool initialize = false) where T : unmanaged 
        => (T*)Alloc(sizeof(T), initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Alloc<T>(uint count, bool initialize = false) where T : unmanaged 
        => (T*)Alloc((uint)sizeof(T) * count, initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* AllocAligned<T>(uint count, bool initialize = false) where T : unmanaged
    {
        Debug.Assert(_context != null);
        //NOTE(Jens): this will align each element to 8 bytes, not just the entire block of memory.
        var alignedSize = MemoryUtils.AlignToUpper(sizeof(T));
        var totalSize = alignedSize * count;
        var mem = Alloc<T>(totalSize, initialize);

        return mem;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray<T> AllocateArray<T>(uint count, bool initialize = false) where T : unmanaged
        => new(Alloc<T>(count, initialize), count);


    public void Reset() => _reset(_context);
    public void Release()
    {
        if (_context != null)
        {
            _release(_context);
            this = default;
        }
    }
}
