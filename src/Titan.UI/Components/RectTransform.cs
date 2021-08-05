using System.Numerics;
using System.Runtime.InteropServices;
using Titan.UI.Common;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RectTransform
    {
        public Vector2 Offset;
        public Size Size;
        public int ZIndex;
        public AnchorPoint AnchorPoint;

        internal Vector2 Position;
        internal int AbsoluteZIndex;
    }
}
