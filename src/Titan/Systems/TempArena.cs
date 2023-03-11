using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Memory;
using Titan.Memory;

namespace Titan.Systems;

public readonly unsafe struct TempArena
{
    private readonly PerFrameArena* _arena;
    internal TempArena(PerFrameArena* arena) => _arena = arena;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanBuffer AllocBuffer(uint size) => new(_arena->Alloc(size), size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray<T> AllocArray<T>(int count) where T : unmanaged
    {
        Debug.Assert(count >= 0);
        return AllocArray<T>((uint)count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray<T> AllocArray<T>(uint count) where T : unmanaged => new(_arena->Alloc((uint)(sizeof(T) * count)), count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(uint size) => _arena->Alloc(size);
}
