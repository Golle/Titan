//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Threading;

//namespace Titan.Memory.Arenas;

//public static class FixedSizeLinearArenaExtensions
//{
//    public static unsafe T* Allocate<T>(this ref FixedSizeLinearArena arena) where T : unmanaged => (T*)arena.Allocate((nuint)sizeof(T));
//}

///// <summary>
///// Fixed size linear arena, this can be used for temporary memory where the buffer is pre-allocated.
///// Thread safe with the use of Interlocked.Add instead of simple increase.
///// Does not own the memory and wont release it
///// </summary>
//public unsafe struct FixedSizeLinearArena
//{
//    private readonly byte* _backingBuffer;
//    private readonly int _size;
//    private volatile int _offset;

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void* Allocate(nuint size)
//    {
//        var alignedSize = (int)size;
//        var offset = Interlocked.Add(ref _offset, alignedSize);
//        Debug.Assert(offset < _size, "Allocation overflow");
//        return _backingBuffer + offset - alignedSize;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void Reset() => _offset = 0;
//    private FixedSizeLinearArena(void* backingBuffer, nuint size)
//    {
//        _backingBuffer = (byte*)backingBuffer;
//        _size = (int)size;
//        _offset = 0;
//    }

//    public static FixedSizeLinearArena Create(void* backingBuffer, nuint size)
//        => new(backingBuffer, size);
//}
