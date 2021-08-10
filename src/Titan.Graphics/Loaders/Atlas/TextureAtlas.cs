using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Loaders.Atlas
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct TextureAtlas
    {
        public Handle<Texture> Texture;
        public MemoryChunk<AtlasDescriptor> Descriptors;
        public MemoryChunk<Vector2> Coordinates;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public ReadOnlySpan<Vector2> Get(int index)
        {
            ref readonly var descriptor = ref Descriptors[index];
            return new ReadOnlySpan<Vector2>(Coordinates.AsPointer() + descriptor.Start, descriptor.Length);
        }
    }
}
