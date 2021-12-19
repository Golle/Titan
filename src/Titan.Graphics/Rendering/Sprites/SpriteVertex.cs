using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Rendering.Sprites
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 Texture;
        public Color Color;
    }
}
