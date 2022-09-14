//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using Titan.Core;

//namespace Titan.Memory.Allocators;

//public unsafe struct ConcurrentLinearDynamicAllocator
//{
//    private VirtualMemory _memory;
//    private volatile uint _offset;
//    private TitanSpinLock _spinLock;

//    public static bool Create(PlatformAllocator* platformAllocator, uint maxSize, out ConcurrentLinearDynamicAllocator allocator)
//    {
//        //TODO(Jens): implement support for shrinking the underlying allocation. could have an "initial size" that it will reset to after each frame (or after X frames)
//        allocator = default;
//        if (!VirtualMemory.Create(platformAllocator, maxSize, out var virtualMemory))
//        {
//            return false;
//        }

//        allocator = new()
//        {
//            _memory = virtualMemory,
//            _spinLock = default
//        };
//        return true;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public T* Allocate<T>() where T : unmanaged => (T*)Allocate((uint)sizeof(T));

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public T* Allocate<T>(uint count) where T : unmanaged => (T*)Allocate((uint)(sizeof(T) * count));

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void* Allocate(int size)
//    {
//        Debug.Assert(size >= 0);
//        return Allocate((uint)size);
//    }

//    public void* Allocate(uint size)
//    {
//        while (_offset + size > _memory.Size)
//        {
//            Expand(size);
//        }

//        var offset = Interlocked.Add(ref _offset, size) - size;
//        var mem = (byte*)_memory.Mem + offset;
//        return mem;
//    }


//    public void Reset()
//    {
//        _offset = 0;
//    }

//    public void Release()
//    {
//        _memory.Release();
//        _offset = 0;
//        _memory = default;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
//    private void Expand(nuint size)
//    {
//        _spinLock.WaitForAccess();
//        if (_offset + size <= _memory.Size)
//        {
//            // some other thread increased the memory while we were waiting, just return.
//            return;
//        }
//        var currentSize = _memory.Size;
//        if (currentSize == 0)
//        {
//            _memory.Resize(size);
//        }
//        else
//        {
//            //NOTE(Jens): double the current size or just increase with whatever size was requested?
//            //_memory.Resize(_memory.Size + size); // Increase with the size requested (will be page aligned)
//            var newSize = Math.Max(_memory.Size * 2, size); // increase with double size.
//            _memory.Resize((nuint)newSize);
//        }
//    }
//}
