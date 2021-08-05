using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Loaders.Atlas
{
    public unsafe class AtlasLoader : IAssetLoader
    {
        private readonly AtlasManager _manager;

        public AtlasLoader(AtlasManager manager)
        {
            _manager = manager;
        }

        public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            Debug.Assert(buffers.Length == 1, "Only a single file can be used for atlas");
            Debug.Assert(dependencies.Length == 1, "Only a single dependency can be used for atlas");

            var atlasFile = buffers[0];

            // TODO: fix this so it doens't allocate extra strings
            using var stream = new StreamReader(new UnmanagedMemoryStream(atlasFile.GetPointer(0), atlasFile.Size), Encoding.UTF8);
            
            if (!uint.TryParse(stream.ReadLine(), out var count))
            {
                throw new InvalidOperationException("Failed to get the texture count");
            }

            var textureHandle = dependencies[0].AssetHandle;
            ref readonly var texture = ref GraphicsDevice.TextureManager.Access(textureHandle);
            var textureWidth = texture.Width;
            var textureHeight = texture.Height;

            var handle = _manager.Create(new AtlasCreation(count));
            ref var atlas = ref _manager.Access(handle);
            atlas.Texture = textureHandle;
            
            var coordinates = stackalloc Vector2[4];
            for (var i = 0; i < count; ++i)
            {
                var values = stream.ReadLine().Split(';');
                if (values.Length < 4)
                {
                    throw new InvalidOperationException("Line with texture coordinates must contain atleast 4 values.");
                }
                
                var x = (float)int.Parse(values[0]);
                var y = (float)int.Parse(values[1]);
                var width = (float)int.Parse(values[2]);
                var height = (float)int.Parse(values[3]);

                coordinates[0] = new Vector2(x/textureWidth, (y+height)/textureHeight);
                coordinates[1] = new Vector2(x/textureWidth, y/textureHeight);
                coordinates[2] = new Vector2((x + width)/textureWidth, y/textureHeight);
                coordinates[3] = new Vector2((x + width) / textureWidth, (y + height) / textureHeight);
                
                atlas.Coordinates[i] = *(TextureCoordinates*)coordinates;
            }

            Logger.Trace<AtlasLoader>($"Loaded Atlas with {count} texture coordinates.");

            return handle;
        }

        public void OnRelease(int handle)
        {
            Logger.Trace<AtlasLoader>($"Releasing Atlas with handle {handle}");
            _manager.Release(handle);
        }

        public void Dispose()
        {
            _manager.Dispose();
        }
    }
    
}
