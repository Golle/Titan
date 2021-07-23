using System.Runtime.InteropServices;
using Titan.UI.Common;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct UITransform
    {
        public Vector2Int Offset;
        public AnchorPoint AnchorPoint;
    }
}
