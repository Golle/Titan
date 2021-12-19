using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Graphics;
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

        // TODO: move this to UI specific code
        public AnchorPoint AnchorPoint;

        // Calculated values
        // TODO: make these internal
        public Vector2 AbsolutePosition;
        public Vector2 AbsolutePivot;
        public Size AbsoluteSize;
        public int AbsoluteZIndex;


        public static RectTransform Create(in Vector2 offset, in Size size) =>
            new()
            {
                Size = size,
                Offset = offset
            };
    }
}
