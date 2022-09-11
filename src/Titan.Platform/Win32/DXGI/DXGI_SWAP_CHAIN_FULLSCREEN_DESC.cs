using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.DXGI
{
    public struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
    {
        public DXGI_RATIONAL RefreshRate;
        public DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
        public DXGI_MODE_SCALING Scaling;
        [MarshalAs(UnmanagedType.Bool)]
        public bool Windowed;
    }
}
