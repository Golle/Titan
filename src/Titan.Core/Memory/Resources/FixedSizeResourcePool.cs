using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.Memory.Resources;

internal unsafe class FixedSizeResourcePool<T> : IResourcePool<T> where T : unmanaged
{
    private static readonly uint Stride = MemoryUtils.AlignToUpper(sizeof(T));
    private static readonly uint HandleOffset = (uint)Random.Shared.Next(2000, 100_000);
    private TitanBuffer _buffer;
    private Header* _freeList;

    private IMemoryManager _memoryManager;
    private SpinLock _spinLock;

    public static bool Create(IMemoryManager memoryManager, uint maxCount, out IResourcePool<T> pool)
    {
        Unsafe.SkipInit(out pool);
        Debug.Assert(maxCount > 0);

        var size = Stride * maxCount;
        var buffer = memoryManager.AllocBuffer(size);
        if (!buffer.HasData())
        {
            Logger.Error<FixedSizeResourcePool<T>>($"Failed to allocate {size} bytes of memory");
            return false;
        }

        // init the free list from the back so that the first element we take out is the first element in the memory
        Header* header = null;
        for (var i = (int)maxCount - 1; i >= 0; --i)
        {
            var next = (Header*)(buffer.AsPointer() + (i * Stride));
            next->Previous = header;
            header = next;
        }

        pool = new FixedSizeResourcePool<T>
        {
            _memoryManager = memoryManager,
            _buffer = buffer,
            _freeList = header
        };
        return true;
    }

    public Handle<T> Alloc()
    {
        var mem = (T*)_freeList;
        if (mem == null)
        {
            Logger.Error<FixedSizeResourcePool<T>>($"Resource pool for type {typeof(T).Name} is full");
            return 0;
        }
        _freeList = _freeList->Previous;
        return PointerToHandle(mem);
    }

    public Handle<T> SafeAlloc()
    {
        //NOTE(Jens): not an optimal way to implement the safe Alloc. but resources should only be created in background systems anyway so it's most likely fine.
        //NOTE(Jens): "background systems anyway" is wrong :) we use this in Audio.
        var gotLock = false;
        _spinLock.Enter(ref gotLock);
        var handle = Alloc();
        _spinLock.Exit();
        return handle;
    }

    public void Free(Handle<T> handle)
    {
        //NOTE(Jens): Add a check that we're not releasing the same hnadle multiple times or that it does nt belong to this resouce pool. Should be behind a debug flag(removed in release builds)
        var header = (Header*)GetPointer(handle);
        header->Previous = _freeList;
        _freeList = header;
    }

    public void SafeFree(Handle<T> handle)
    {
        var gotLock = false;
        _spinLock.Enter(ref gotLock);
        Free(handle);
        _spinLock.Exit();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(Handle<T> handle)
        => ref *GetPointer(handle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* GetPointer(Handle<T> handle)
        => (T*)HandleToPointer(handle);

    public void Release()
    {
        if (_memoryManager != null)
        {
            _memoryManager.Free(ref _buffer);
            _buffer = default;
            _freeList = null;
            _memoryManager = null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Handle<T> PointerToHandle(void* ptr)
    {
        //NOTE(Jens): Add more verifications that should only be done in debug. 
        var ptrAsNumber = (nuint)ptr;
        var bufferAsNumber = (nuint)_buffer.AsPointer();
        var diff = ptrAsNumber - bufferAsNumber;
        Debug.Assert(diff >= 0);
        Debug.Assert(diff < uint.MaxValue);
        return (uint)diff + HandleOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void* HandleToPointer(in Handle<T> handle)
    {
        //NOTE(Jens): Add more verifications that should only be done in debug.  For example that the pointer is within the buffer (and correctly aligned). Also make sure that the item hasn't already been freed.
        var offset = (nuint)(handle.Value - HandleOffset);
        var bufferAsValue = (nuint)_buffer.AsPointer();
        var diff = bufferAsValue + offset;
        Debug.Assert(diff >= 0);
        return (void*)diff;
    }

    private struct Header
    {
        public Header* Previous;
    }
}
