using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Graphics.Loaders.Atlas
{
    public readonly record struct AtlasCreation(uint TextureCount, uint NineSliceCount);

    public class AtlasManager : IDisposable
    {
        private ResourcePool<TextureAtlas> _resources;
        public AtlasManager(uint maxAtlases)
        {
            _resources.Init(maxAtlases);
        }

        public unsafe Handle<TextureAtlas> Create(in AtlasCreation args)
        {
            var handle = _resources.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Atlas Handle");
            }

            var atlas = _resources.GetResourcePointer(handle);

            var totalCoordinates = args.NineSliceCount * 16 + args.TextureCount * 4;
            atlas->Coordinates = MemoryUtils.AllocateBlock<Vector2>(totalCoordinates);
            atlas->Descriptors = MemoryUtils.AllocateBlock<AtlasDescriptor>(args.NineSliceCount + args.TextureCount);
            
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TextureAtlas Access(in Handle<TextureAtlas> handle) => ref _resources.GetResourceReference(handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release(in Handle<TextureAtlas> handle)
        {
            Logger.Trace<AtlasManager>($"Releasing atlas with handle {handle}");
            ref var atlas = ref _resources.GetResourceReference(handle.Value);
            atlas.Coordinates.Free();
            atlas.Descriptors.Free();
            _resources.ReleaseResource(handle);
        }

        public unsafe void Dispose()
        {
            Logger.Trace<AtlasManager>("Terminate resource pool");
            foreach (var resource in _resources.EnumerateUsedResources())
            {
                var atlas = _resources.GetResourcePointer(resource.Value);
                atlas->Coordinates.Free();
                atlas->Descriptors.Free();
            }
            _resources.Terminate();
        }
    }
}
