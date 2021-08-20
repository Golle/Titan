using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.UI.Rendering
{
    internal enum RenderableType
    {
        Sprite,
        NineSlice,
        Text
    }

    [SkipLocalsInit]
    [DebuggerDisplay("{Key}")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct SortableRenderable
    {
        public readonly long Key;
        public readonly RenderableType Type;
        public readonly int Index;
        public readonly Handle<Texture> Texture;
        public SortableRenderable(int zIndex, Handle<Texture> texture, int index, RenderableType type)
        {
            Key = (long)zIndex << 32 | (long)texture.Value;
            Index = index;
            Type = type;
            Texture = texture;
        }
    }
}
