using System.Diagnostics;
using Titan.Graphics.D3D11;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;


using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.Graphics.Pipeline.Graph
{
    internal class BufferManager : IBufferManager
    {
        private readonly IGraphicsDevice _device;
        private readonly IWindow _window;

        public BufferManager(IGraphicsDevice device, IWindow window)
        {
            _device = device;
            _window = window;
        }

        public DepthStencil GetDepthStencil(DXGI_FORMAT format = DXGI_FORMAT_R24G8_TYPELESS, D3D11_BIND_FLAG bindFlag = D3D11_BIND_DEPTH_STENCIL, DXGI_FORMAT shaderResourceFormat = DXGI_FORMAT_UNKNOWN)
        {
            Debug.Assert((bindFlag & D3D11_BIND_DEPTH_STENCIL) != 0, "BindFlag must contain D3D11_BIND_DEPTH_STENCIL");

            var resource = new Texture2D(_device, (uint)_window.Width, (uint)_window.Height, format, bindFlag);
            //var shaderResourceView = (bindFlag & D3D11_BIND_SHADER_RESOURCE) != 0 ? new ShaderResourceView(_device, resource, shaderResourceFormat) : null;
            var depthStencilView = new DepthStencilView(_device, resource);
            return new DepthStencil(depthStencilView, resource);
        }
    }
}
