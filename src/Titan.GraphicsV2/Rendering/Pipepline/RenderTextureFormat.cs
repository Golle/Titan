using Titan.Windows.Win32.D3D11;
// ReSharper disable InconsistentNaming

namespace Titan.GraphicsV2.Rendering.Pipepline
{
    internal enum RenderTextureFormat : uint
    {
        None,
        RG32F = DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT,
        RGB32F = DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT,
        RGBA32F = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT,
    }
}
