//using System.Runtime.CompilerServices;
//using System.Threading;

//namespace Titan.Memory.Arenas;
//public static class DynamicLinearArenaExtensions
//{
//    public static unsafe T* Allocate<T>(this ref DynamicLinearArena arena) where T : unmanaged => (T*)arena.Allocate((nuint)sizeof(T));
//}

//public unsafe struct DynamicLinearArena
//{
//    private static readonly nuint FooterSize = (nuint)sizeof(Footer);
//    private readonly Allocator* _allocator;
//    private readonly byte* _baseMemory;
//    private byte* _current;
//    private volatile int _offset; // the memory offset in the current region
//    private readonly int _blockSize; // A block is the entire region of the memory

//    public void* Allocate(nuint size)
//    {
//        var alignedSize = (int)size; // Add alignment
//        if (alignedSize + _offset >= _blockSize)
//        {
//            Expand();
//        }
//        var offset = Interlocked.Add(ref _offset, alignedSize) - alignedSize; // Increment the _offset and subtract the size to get the start of the block
//        return _current + offset;
//    }

//    public void Reset()
//    {
//        // Reset the offset and set the current memory pointer to the current block
//        _offset = 0;
//        _current = _baseMemory;
//        var footer = GetFooter(_baseMemory);

//        //NOTE(Jens): Should we release any newly allocated blocks or just re-use them when/if needed again?
//        RecursiveRelease(footer->Next);
//        footer->Next = null;
//    }

//    private void RecursiveRelease(byte* current)
//    {
//        while (current != null)
//        {
//            var next = GetFooter(current)->Next;
//            _allocator->Free(current);
//            current = next;
//        }
//    }

//    private DynamicLinearArena(Allocator* allocator, nuint blockSize)
//    {
//        _allocator = allocator;
//        _blockSize = (int)blockSize;
//        _offset = 0;
//        _current = _baseMemory = (byte*)_allocator->Allocate(blockSize + FooterSize); // TODO: Align this
//        GetFooter(_current)->Next = null;
//    }

//    public static DynamicLinearArena Create(in PlatformAllocator allocator, nuint initialSize)
//        => Create(allocator.UnderlyingAllocator, initialSize);
//    internal static DynamicLinearArena Create(Allocator* allocator, nuint initialSize)
//        => new(allocator, initialSize);

//    private void Expand()
//    {
//        _offset = 0;
//        var mem = (byte*)_allocator->Allocate((nuint)_blockSize + FooterSize);
//        GetFooter(_current)->Next = mem;
//        // Make sure the Next pointer is set to null. The underlying Allocator might not zero the memory
//        GetFooter(mem)->Next = null;
//        _current = mem;
        
//    }

//    public void Release()
//        => RecursiveRelease(_baseMemory);

//    //NOTE(Jens): The footer is used to track the info we need for new blocks
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    private Footer* GetFooter(byte* ptr) => (Footer*)(ptr + _blockSize);
//    private struct Footer
//    {
//        public byte* Next;
//    }
//}
