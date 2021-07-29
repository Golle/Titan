using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.UI.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct UIVertex
    {
        public Vector2 Position;
        public Vector2 Texture;
    }
}
