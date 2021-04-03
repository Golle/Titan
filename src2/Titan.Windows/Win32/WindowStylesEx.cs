using System;

namespace Titan.Windows.Win32
{
    [Flags]
    internal enum WindowStylesEx : uint
    {
        AcceptFiles = 0x00000010,
        AppWindow = 0x00040000,
        ClientEdge = 0x00000200,
        Composited = 0x02000000,
        ContextHelp = 0x00000400,
        ControlParent = 0x00010000,
        DlgModalFrame = 0x00000001,
        Layered = 0x00080000,
        LayoutRtl = 0x00400000,
        Left = 0x00000000,
        LeftScrollbar = 0x00004000,
        LtrReading = 0x00000000,
        MdiChild = 0x00000040,
        NoActivate = 0x08000000,
        NoInheritLayout = 0x00100000,
        NoParentNotify = 0x00000004,
        NoRedirectionBitmap = 0x00200000,
        OverlappedWindow = (WindowEdge | ClientEdge),
        PaletteWindow = (WindowEdge | ToolWindow | Topmost),
        Right = 0x00001000,
        RightScrollbar = 0x00000000,
        RtlReading = 0x00002000,
        StaticEdge = 0x00020000,
        ToolWindow = 0x00000080,
        Topmost = 0x00000008,
        Transparent = 0x00000020,
        WindowEdge = 0x00000100
    }
}
