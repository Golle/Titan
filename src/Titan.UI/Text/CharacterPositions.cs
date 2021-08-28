using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.UI.Text
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CharacterPositions
    {
        public Vector2 BottomLeft;
        public Vector2 TopRight;
        public char Value;
    }
}
