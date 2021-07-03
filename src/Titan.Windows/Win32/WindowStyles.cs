using System;

namespace Titan.Windows.Win32
{
    [Flags]
    public enum WindowStyles : uint
    {
        Border = 0x00800000,
        Caption = 0x00C00000,
        Child = 0x40000000,
        ChildWindow = 0x40000000,
        ClipChildren = 0x02000000,
        ClipSiblings = 0x04000000,
        Disabled = 0x08000000,
        DlgFrame = 0x00400000,
        Group = 0x00020000,
        HScroll = 0x00100000,
        Iconic = 0x20000000,
        Maximize = 0x01000000,
        MaximizeBox = 0x00010000,
        Minimize = 0x20000000,
        MinimizeBox = 0x00020000,
        Overlapped = 0x00000000,
        OverlappedWindow = (Overlapped | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox),
        Popup = 0x80000000,
        PopupWindow = (Popup | Border | SysMenu),
        SizeBox = 0x00040000,
        SysMenu = 0x00080000,
        TabStop = 0x00010000,
        ThickFrame = 0x00040000,
        Tiled = 0x00000000,
        TiledWindow = (Overlapped | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox),
        Visible = 0x10000000,
        VScroll = 0x00200000
    }
}
