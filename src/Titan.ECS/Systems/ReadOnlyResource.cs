using System.Runtime.CompilerServices;

namespace Titan.ECS.Systems;

public readonly unsafe struct ReadOnlyResource<T> where T : unmanaged
{
    private readonly T* _resource;

    internal ReadOnlyResource(T* resource)
    {
        _resource = resource;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get() => ref *_resource;
}
