using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Pipepline
{
    internal enum DepthTextureFormat : uint
    {
        None,
        D16 = DXGI_FORMAT.DXGI_FORMAT_D16_UNORM,
        D24S8 = DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT,
        D32 = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,
        D32S8X24 = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT
    }
}
