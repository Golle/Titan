using Titan.Core;

namespace Titan.Assets.NewAssets;


//NOTE(Jens): not a very "user friendly" implementation of resource creation. but it will do for now :)
//NOTE(Jens): look into alternative ways to implement this
public unsafe interface IResourceCreator<T> where T : unmanaged
{
    static abstract Handle<T> Create(void* context, ReadOnlySpan<byte> data);
    static abstract void Destroy(void* context, Handle<T> handle);
}
