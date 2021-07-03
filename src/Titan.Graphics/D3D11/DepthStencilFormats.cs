using Titan.Windows.D3D11;
// ReSharper disable InconsistentNaming

namespace Titan.Graphics.D3D11
{
    public enum DepthStencilFormats : uint
    {
        Unknown = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
        D16 = DXGI_FORMAT.DXGI_FORMAT_D16_UNORM,
        D24S8 = DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT,
        D32 = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,
        D32S8X24 = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT
    }
}
