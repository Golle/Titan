using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Graphics.Loaders.Atlas
{
    public record AtlasCreation(uint TextureCount);

    public class AtlasManager
    {
        private ResourcePool<TextureAtlas> _resources;

        public AtlasManager(uint maxAtlases)
        {
            _resources.Init(maxAtlases);
        }

        public unsafe Handle<TextureAtlas> Create(AtlasCreation args)
        {
            var handle = _resources.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Atlas Handle");
            }

            var atlas = _resources.GetResourcePointer(handle);
            
            atlas->Coordinates = MemoryUtils.AllocateBlock<TextureCoordinates>(args.TextureCount);
         
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
            _resources.ReleaseResource(handle);
        }

        public void Dispose()
        {
            Logger.Trace<AtlasManager>("Terminate resource pool");
            _resources.Terminate();
        }
    }
}
