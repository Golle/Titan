using static Titan.Windows.D3D11.DXGI_FORMAT;

// ReSharper disable InconsistentNaming

namespace Titan.Graphics.D3D11.Shaders
{
    public enum TextureFormats : uint
    {
        None,
        RG32F = DXGI_FORMAT_R32G32_FLOAT,
        RGB32F = DXGI_FORMAT_R32G32B32_FLOAT,
        RGBA32F = DXGI_FORMAT_R32G32B32A32_FLOAT,
    }
}
