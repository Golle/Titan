using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct InteractableComponent
    {
        public Vector2 TopLeft;
        public Vector2 BottomRight;

        public MouseState MouseState;
    }
}
