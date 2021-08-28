using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Loaders.Atlas
{

    public enum SpriteType : byte
    {
        Normal = 0,
        Slice = 1
    }
    public unsafe class AtlasLoader : IAssetLoader
    {
        private readonly AtlasManager _manager;

        private readonly int _textureCoordinateSize = sizeof(Vector2) * 4;
        private readonly int _nineSliceCoordinateSize = sizeof(Vector2) * 16;
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
            var counts = stream.ReadLine().Split(';');

            var textureCount = uint.Parse(counts[0]);
            var nineSliceCount = uint.Parse(counts[1]);

            var textureHandle = dependencies[0].AssetHandle;
            ref readonly var texture = ref GraphicsDevice.TextureManager.Access(textureHandle);
            var textureWidth = (float)texture.Width;
            var textureHeight = (float)texture.Height;

            var handle = _manager.Create(new AtlasCreation(textureCount, nineSliceCount));
            ref var atlas = ref _manager.Access(handle);
            atlas.Texture = textureHandle;

            var coordinates = stackalloc Vector2[16];
            
            var descriptorIndex = 0;
            var coordinateIndex = 0;

            
            for (var i = 0; i < textureCount; ++i)
            {
                var values = stream.ReadLine().Split(';');

                var x = (float)int.Parse(values[0]);
                var y = (float)int.Parse(values[1]);
                var width = (float)int.Parse(values[2]);
                var height = (float)int.Parse(values[3]);

                coordinates[0] = new Vector2(x / textureWidth, (y + height) / textureHeight);
                coordinates[1] = new Vector2(x / textureWidth, y / textureHeight);
                coordinates[2] = new Vector2((x + width) / textureWidth, y / textureHeight);
                coordinates[3] = new Vector2((x + width) / textureWidth, (y + height) / textureHeight);
                
                Buffer.MemoryCopy(coordinates, atlas.Coordinates.AsPointer() + coordinateIndex, _textureCoordinateSize, _textureCoordinateSize);

                atlas.Descriptors[descriptorIndex++] = new AtlasDescriptor
                {
                    Start = (byte)coordinateIndex,
                    Length = 4,
                    Type = SpriteType.Normal
                };
                coordinateIndex += 4;
            }

            var coords = stackalloc Vector2[4];
            for (var i = 0; i < nineSliceCount; ++i)
            {
                var values = stream.ReadLine().Split(';');
                var x = (float)int.Parse(values[0]);
                var y = (float)int.Parse(values[1]);
                var width = (float)int.Parse(values[2]);
                var height = (float)int.Parse(values[3]);
                var top = (float)int.Parse(values[4]);
                var bottom = (float)int.Parse(values[5]);
                var left = (float)int.Parse(values[6]);
                var right = (float)int.Parse(values[7]);

                // Calculate all needed coordinates
                coords[0] = new Vector2(x / textureWidth, (y + height) / textureHeight);
                coords[1] = new Vector2((x + left) / textureWidth, (y + height - bottom) / textureHeight);
                coords[2] = new Vector2((x + width - right) / textureWidth, (y + top) / textureHeight);
                coords[3] = new Vector2((x + width) / textureWidth, y / textureHeight);
                
                // Map the coordinates to each vertex
                var index = 0;
                for (var row = 0; row < 4; ++row)
                {
                    for (var col = 0; col < 4; ++col)
                    {
                        coordinates[index++] = new Vector2(coords[col].X, coords[row].Y);
                    }
                }
                
                Buffer.MemoryCopy(coordinates, atlas.Coordinates.AsPointer() + coordinateIndex, _nineSliceCoordinateSize, _nineSliceCoordinateSize);

                atlas.Descriptors[descriptorIndex++] = new AtlasDescriptor
                {
                    Length = 16,
                    Start = (byte)coordinateIndex,
                    Type = SpriteType.Slice
                };

                coordinateIndex += 16;
            }
            Logger.Trace<AtlasLoader>($"Loaded Atlas with {textureCount} sprites and {nineSliceCount} nine slice sprites.");

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
