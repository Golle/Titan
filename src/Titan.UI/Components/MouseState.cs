using System;

namespace Titan.UI.Components
{
    [Flags]
    public enum MouseState
    {
        None = 0x0,
        Hover = 0x1,
        Down = 0x2,
        Up = 0x4
    }
}
