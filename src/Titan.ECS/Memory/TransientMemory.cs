using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Memory.Arenas;

namespace Titan.ECS.Memory;

public unsafe struct TransientMemory : IApi
{
    private FixedSizeLinearArena _arena;
    public TransientMemory(FixedSizeLinearArena arena) => _arena = arena;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* AllocatePointer<T>(bool initialize = false) where T : unmanaged 
        => AllocateArray<T>(1, initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* AllocateArray<T>(uint count, bool initialize = false) where T : unmanaged
    {
        var size = (uint)(sizeof(T) * count);
        var ptr = (T*)_arena.Allocate(size);
        if (initialize)
        {
            Unsafe.InitBlockUnaligned(ptr, 0, size);
        }
        return ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Allocate<T>(bool initialize = false) where T : unmanaged
        => ref *AllocatePointer<T>(initialize);

    /// <summary>
    /// Should only be called by internal systems. Will reset the current memory pool
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() => _arena.Reset();
}
