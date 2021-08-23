using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Loaders.Fonts
{
    public struct Font
    {
        public int Offset;
        public Handle<Texture> Texture;
        internal MemoryChunk<Glyph> Glyphs;
        public ushort FontSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public ref readonly Glyph Get(char c)
        {
            Debug.Assert(c >= Offset && c < Glyphs.Count, $"Character {c} is out of range.");
            return ref Glyphs[c - Offset];
        }
    }
}
