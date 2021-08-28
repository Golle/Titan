using System.Numerics;
using System.Runtime.InteropServices;
using Titan.UI.Common;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RectTransform
    {
        public Vector2 Offset;
        public Vector2 Pivot;
        public Size Size;
        public int ZIndex;
        public AnchorPoint AnchorPoint;

        // Calculated values
        internal Vector2 AbsolutePosition;
        internal Vector2 AbsolutePivot;
        internal Size AbsoluteSize;
        internal int AbsoluteZIndex;
    }
}
