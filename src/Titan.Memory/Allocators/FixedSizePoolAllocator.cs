using System.Diagnostics;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators;

public unsafe struct FixedSizePoolAllocator<T> where T : unmanaged
{
    private static readonly uint Size = (uint)sizeof(T);
    private static readonly uint AlignedSize = MemoryUtils.AlignToUpper((uint)sizeof(T));
    private readonly GeneralAllocator* _allocator;
    private T* _mem;
    private Header* _freeList;
    private readonly uint _maxCount;

    public int _count;
    private FixedSizePoolAllocator(GeneralAllocator* allocator, void* mem, uint maxCount)
    {
        _allocator = allocator;
        _maxCount = maxCount;
        _mem = (T*)mem;

        Header* header = null;
        for (var i = (int)_maxCount - 1; i >= 0; --i)
        {
            var next = (Header*)(_mem + i);
            next->Previous = header;
            header = next;
        }
        _freeList = header;
    }

    public static bool Create(GeneralAllocator* allocator, uint maxCount, out FixedSizePoolAllocator<T> poolAllocator)
        => Create(allocator, maxCount, true, out poolAllocator);

    public static bool Create(GeneralAllocator* allocator, uint maxCount, bool alignedPerObject, out FixedSizePoolAllocator<T> poolAllocator)
    {
        poolAllocator = default;

        var typeSize = alignedPerObject ? AlignedSize : Size;
        var mem = allocator->Allocate(typeSize * maxCount);
        if (mem == null)
        {
            Console.WriteLine($"Failed to allocate {typeSize * maxCount} bytes of memory.");
            return false;
        }
        Debug.Assert(mem != null);
        poolAllocator = new(allocator, mem, maxCount);

        return true;
    }

    public T* Allocate()
    {
        Debug.Assert(_freeList != null, "Pool max size reached.");
        var mem = _freeList;
        _freeList = _freeList->Previous;
        _count++;
        return (T*)mem;
    }

    public void Free(T* ptr)
    {
        //NOTE(Jens): this allows multiple frees of the same object. How can that be prevented?
        var header = (Header*)ptr;
        header->Previous = _freeList;
        _freeList = header;
        _count--;
    }

    public void Release()
    {
        if (_mem != null)
        {
            _allocator->Free(_mem);
            _mem = null;
        }
    }

    private struct Header
    {
        public Header* Previous;
    }
}
