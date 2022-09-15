using Titan.Platform.Win32.Win32;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.DXGI
{
    public unsafe struct DXGI_PRESENT_PARAMETERS
    {
        public uint DirtyRectsCount;
        public RECT *pDirtyRects;
        public RECT *pScrollRect;
        public POINT *pScrollOffset;
    }
}
