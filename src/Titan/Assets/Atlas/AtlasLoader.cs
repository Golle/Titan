using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Assets.Atlas
{

    [StructLayout(LayoutKind.Sequential, Size = sizeof(float) * 8)]
    public unsafe struct TextureCoordinates
    {
        public ref readonly Vector2 this[int index]
        {
            get
            {
                Debug.Assert(index < 4, "Index must be between 0 and 4");
                return ref *((Vector2*)Unsafe.AsPointer(ref this) + index);
            }
        }
    }
    public struct TextureAtlas
    {
        public Handle<Texture> Texture;
        public MemoryChunk<TextureCoordinates> Coordinates;
    }

    public unsafe class AtlasLoader : IAssetLoader
    {

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

            // Create a manager for this, and return a handle for the atlas
            var atlas = new TextureAtlas
            {
                Texture = textureHandle,
                Coordinates = MemoryUtils.AllocateBlock<TextureCoordinates>(count)
            };

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


            return 0;
        }

        public void OnRelease(int handle)
        {

        }

        public void Dispose()
        {
        }
    }
    
}
