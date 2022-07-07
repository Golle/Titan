using System.Runtime.CompilerServices;

namespace Titan.ECS.SystemsV2;

public readonly unsafe struct ApiResource<T> where T : unmanaged
{
    private readonly T* _api;
    internal ApiResource(T* api) => _api = api;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get() => ref *_api;
}
