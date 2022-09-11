//using System.Diagnostics;
//using System.Threading;
//using Titan.Core.Logging;
//using Titan.Core.Memory;

//namespace Titan.Memory.Arenas;

//public static class DynamicPoolArenaExtensions
//{
//    public static unsafe T* Allocate<T>(this ref DynamicPoolArena arena) where T : unmanaged => (T*)arena.Allocate();
//}

//public unsafe struct DynamicPoolArena
//{
//    private PlatformAllocator* _allocator; // used to free and expand the pool
//    private byte* _mem;
//    private Block* _freeList;
//    private ushort _blockSize;
//    private ushort _count;

//    private volatile int _lock;
//    public static DynamicPoolArena Create(PlatformAllocator* allocator, uint count, uint blockSize) =>
//        new()
//        {
//            _allocator = allocator,
//            _blockSize = (ushort)blockSize,
//            _count = (ushort)count,
//            _freeList = null,
//            _mem = null
//        };

//    public void* Allocate(bool zeroMemory = true)
//    {
//        if (_freeList == null)
//        {
//            Expand();
//        }
//        var nextBlock = _freeList;
//        Debug.Assert(nextBlock != null, "Not sure what happened here");
//        _freeList = _freeList->Next;
//        if (zeroMemory)
//        {
//            MemoryUtils.Init(nextBlock, (uint)_blockSize);
//        }
//        return nextBlock;
//    }

//    /// <summary>
//    /// Thread safe version of Allocate that uses CompareaExchange and SpinWait for synchronization
//    /// </summary>
//    /// <param name="zeroMemory"></param>
//    /// <returns></returns>
//    public void* SafeAllocate(bool zeroMemory = true)
//    {
//        WaitForAccess();
//        var mem = Allocate(zeroMemory);
//        _lock = 0;
//        return mem;
//    }

//    /// <summary>
//    /// Thread safe version of Free that uses CompareaExchange and SpinWait for synchronization
//    /// </summary>
//    /// <param name="ptr">The pointer to the memory allocated in this pool</param>
//    public void SafeFree(void* ptr)
//    {
//        Logger.Info($"SafeFree - Wait");
//        WaitForAccess();
//        Logger.Info($"SafeFree - Free");
//        Free(ptr);
//        Logger.Info($"SafeFree - Free Completed");
//        _lock = 0;
//    }


//    public void Free(void* ptr)
//    {
//        IsInRange(ptr);
//        var block = ((Block*)ptr);
//        block->Next = _freeList;
//        _freeList = block;
//    }

//    private void WaitForAccess()
//    {
//        var sw = new SpinWait();
//        while (true)
//        {
//            if (Interlocked.CompareExchange(ref _lock, 1, 0) == 0)
//            {
//                break;
//            }
//            sw.SpinOnce();
//        }
//    }

//    public void Release()
//    {
//        var mem = _mem;
//        do
//        {
//            var prev = GetFooter(mem)->PreviousBlock;
//            _allocator->Free(mem);
//            mem = prev;
//        } while (mem != null);
//    }

//    private void Expand()
//    {
//        var footerSize = sizeof(Footer);
//        var poolSize = _blockSize * _count;
//        var mem = (byte*)_allocator->Allocate((nuint)(poolSize + footerSize)); //We could enforce 0 alloc
//        for (var i = 0; i < _count - 1; ++i)
//        {
//            var block = (Block*)(mem + _blockSize * i);
//            block->Next = (Block*)(mem + _blockSize * (i + 1));
//        }
//        // Set the last block next pointer to null in case the allocator does not return initialized memory
//        ((Block*)(mem + _blockSize * (_count - 1)))->Next = null;
//        var footer = (Footer*)(mem + poolSize);
//        footer->PreviousBlock = _mem;
//        _mem = mem;
//        _freeList = (Block*)_mem;
//    }

//    private struct Block
//    {
//        public Block* Next;
//    }

//    private struct Footer
//    {
//        public byte* PreviousBlock;
//    }

//    [Conditional("DEBUG")]
//    private void IsInRange(void* ptr)
//    {
//        var memory = _mem;
//        do
//        {
//            if (ptr >= memory && ptr <= memory + (_count * _blockSize))
//            {
//                return;
//            }
//            memory = GetFooter(memory)->PreviousBlock;
//        } while (memory != null);
//        Debug.Assert(false, "Trying to free a block of memory not allocated by this pool");
//    }

//    private Footer* GetFooter(byte* mem) => (Footer*)(mem + (_blockSize * _count));
//}
