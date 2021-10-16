using static Titan.Windows.DXGI.DXGI_FORMAT;

// ReSharper disable InconsistentNaming

namespace Titan.Graphics.D3D11
{
    public enum TextureFormats : uint
    {
        None,
        RG32F = DXGI_FORMAT_R32G32_FLOAT,
        RGB32F = DXGI_FORMAT_R32G32B32_FLOAT,
        RGBA32F = DXGI_FORMAT_R32G32B32A32_FLOAT,

        RGBA16F = DXGI_FORMAT_R16G16B16A16_FLOAT,
        RGBA16U = DXGI_FORMAT_R16G16B16A16_UNORM,

        RGBA8U = DXGI_FORMAT_R8G8B8A8_UNORM,
        BGRA8U = DXGI_FORMAT_B8G8R8A8_UNORM,
        BGRA8USRGB = DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,
        // Not sure these are needed.
        //BGRA8U = DXGI_FORMAT_B8G8R8A8_UNORM, // DXGI 1.1
        //BGRX8U = DXGI_FORMAT_B8G8R8X8_UNORM, // DXGI 1.1

        //DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM, // DXGI 1.1
        //DXGI_FORMAT_R10G10B10A2_UNORM,
        //DXGI_FORMAT_R9G9B9E5_SHAREDEXP,
        //DXGI_FORMAT_B5G5R5A1_UNORM,
        //DXGI_FORMAT_B5G6R5_UNORM,
        R32U = DXGI_FORMAT_R32_UINT,
        R32F = DXGI_FORMAT_R32_FLOAT,
        R32TL = DXGI_FORMAT_R32_TYPELESS,
        R16F = DXGI_FORMAT_R16_FLOAT,
        R16U = DXGI_FORMAT_R16_UNORM,
        R16TL = DXGI_FORMAT_R16_TYPELESS,
        R8U = DXGI_FORMAT_R8_UNORM,
        A8U = DXGI_FORMAT_A8_UNORM,
        R24G8TL = DXGI_FORMAT_R24G8_TYPELESS,
        R32FG8X24TL = DXGI_FORMAT_R32G8X24_TYPELESS
    }
}
