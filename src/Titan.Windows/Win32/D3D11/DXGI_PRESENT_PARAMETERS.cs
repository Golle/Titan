using Titan.Windows.Win32.Native;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.D3D11
{
    public unsafe struct DXGI_PRESENT_PARAMETERS
    {
        public uint DirtyRectsCount;
        public RECT *pDirtyRects;
        public RECT *pScrollRect;
        public POINT *pScrollOffset;
    }
}
