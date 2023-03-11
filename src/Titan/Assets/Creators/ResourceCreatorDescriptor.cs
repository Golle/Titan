using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Setup.Configs;

namespace Titan.Assets.Creators;

internal record ResourceCreatorConfiguration(ResourceCreatorDescriptor Descriptor) : IConfiguration
{
    public static ResourceCreatorConfiguration Create<TResource, TContext>() where TResource : unmanaged where TContext : unmanaged, IResourceCreator<TResource>
        => new(ResourceCreatorDescriptor.CreateDescriptor<TResource, TContext>());
}

internal unsafe struct ResourceCreatorDescriptor
{
    public delegate*<void*, in ResourceCreatorInitializer, bool> Init;
    public delegate*<void*, void> Release;
    public delegate*<void*, in AssetDescriptor, TitanBuffer, Handle> Create;
    public delegate*<void*, in Handle, in AssetDescriptor, TitanBuffer, bool> Recreate;
    public delegate*<void*, Handle, void> Destroy;
    public uint ContextSize;
    public AssetDescriptorType Type;

    public static ResourceCreatorDescriptor CreateDescriptor<TResource, TContext>()
        where TResource : unmanaged
        where TContext : unmanaged, IResourceCreator<TResource> =>
        new()
        {
            Type = TContext.Type,
            ContextSize = (uint)sizeof(TContext),
            Init = &FunctionWrapper<TResource, TContext>.Init,
            Create = &FunctionWrapper<TResource, TContext>.Create,
            Recreate = &FunctionWrapper<TResource, TContext>.Recreate,
            Destroy = &FunctionWrapper<TResource, TContext>.Destroy,
            Release = &FunctionWrapper<TResource, TContext>.Release
        };

    /// <summary>
    /// This struct is used to map static functions to specific instances, making the usage of the interface a bit more friendly.
    /// </summary>
    /// <typeparam name="TResource">The resource type that is created</typeparam>
    /// <typeparam name="TContext">The struct implementing this interface</typeparam>
    private struct FunctionWrapper<TResource, TContext>
        where TContext : unmanaged, IResourceCreator<TResource>
        where TResource : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Init(void* context, in ResourceCreatorInitializer initializer) => ((TContext*)context)->Init(initializer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release(void* context) => ((TContext*)context)->Release();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Handle Create(void* context, in AssetDescriptor descriptor, TitanBuffer data) => ((TContext*)context)->Create(descriptor, data).Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Recreate(void* context, in Handle handle, in AssetDescriptor descriptor, TitanBuffer data) => ((TContext*)context)->Recreate((Handle<TResource>)handle, descriptor, data);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(void* context, Handle handle) => ((TContext*)context)->Destroy((Handle<TResource>)handle);
    }
}
