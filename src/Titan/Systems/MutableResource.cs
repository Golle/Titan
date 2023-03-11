using System.Runtime.CompilerServices;

namespace Titan.Systems;

public readonly unsafe struct MutableResource<T> where T : unmanaged
{
    private readonly T* _resource;
    internal MutableResource(T* resource) => _resource = resource;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get() => ref *_resource;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T* AsPointer() => _resource;
}
