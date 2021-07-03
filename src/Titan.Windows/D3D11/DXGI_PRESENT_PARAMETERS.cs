using Titan.Windows.Win32;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11
{
    public unsafe struct DXGI_PRESENT_PARAMETERS
    {
        public uint DirtyRectsCount;
        public RECT *pDirtyRects;
        public RECT *pScrollRect;
        public POINT *pScrollOffset;
    }
}
