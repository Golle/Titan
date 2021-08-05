using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Loaders.Atlas
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureAtlas
    {
        public Handle<Texture> Texture;
        public MemoryChunk<TextureCoordinates> Coordinates;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public ref readonly TextureCoordinates Get(int index)
        {
            Debug.Assert(index >= 0 && index < Coordinates.Count, "Index it out of range.");
            return ref Coordinates[index];
        }
    }
}
