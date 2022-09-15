using Titan.Platform.Win32.D3D11;

namespace Titan.Graphics.D3D11.Rasterizer
{
    public enum CullMode
    {
        None = D3D11_CULL_MODE.D3D11_CULL_NONE,
        Front = D3D11_CULL_MODE.D3D11_CULL_FRONT,
        Back = D3D11_CULL_MODE.D3D11_CULL_BACK
    }
}
