using System;

namespace Titan.UI.Common
{
    [Flags]
    public enum AnchorPoint 
    {
        Default = 0,

        BottomLeft = Bottom|Left,
        BottomCenter = Bottom|Center,
        BottomRight = Bottom|Right,
        MiddleRight = Middle|Right,
        MiddleCenter = Middle|Center,
        MiddleLeft = Middle|Left,
        TopLeft = Top|Left,
        TopCenter = Top|Center,
        TopRight = Top|Right,

        
        Left = 1,
        Right = 2,
        Center = 4,
        Top = 8,
        Middle = 16,
        Bottom = 32,


        HorizontalMask = Left|Right|Center,
        VerticalMask = Top|Bottom|Middle
        // TODO: add support for stretch
    }
}
