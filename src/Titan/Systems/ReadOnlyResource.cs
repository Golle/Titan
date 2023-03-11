using System.Runtime.CompilerServices;

namespace Titan.Systems;

public readonly unsafe struct ReadOnlyResource<T> where T : unmanaged
{
    private readonly T* _resource;
    internal ReadOnlyResource(T* resource) => _resource = resource;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get() => ref *_resource;


    /// <summary>
    /// Method that is only available inside the engine for performnace reasons
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T* AsPointer() => _resource;
}
