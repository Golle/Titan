using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.GraphicsV2.Rendering.Renderers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FullscreenVertex
    {
        internal Vector2 Position;
        internal Vector2 UV;
    }
}
