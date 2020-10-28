using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.Graphics.Pipeline.Graph
{
    public interface IBufferManager
    {
        RenderBuffer GetBuffer(DXGI_FORMAT format, D3D11_BIND_FLAG bindFlag, float width = 1f, float height = 1f);
        DepthStencil GetDepthStencil(DXGI_FORMAT format = DXGI_FORMAT_R24G8_TYPELESS, D3D11_BIND_FLAG bindFlag = D3D11_BIND_DEPTH_STENCIL);
    }
}
