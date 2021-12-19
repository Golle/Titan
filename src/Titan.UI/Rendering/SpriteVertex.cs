using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Graphics;

namespace Titan.UI.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 Texture;
        public Color Color;
    }
}
