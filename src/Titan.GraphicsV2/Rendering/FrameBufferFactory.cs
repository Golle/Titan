using System;
using System.Linq;
using Titan.GraphicsV2.D3D11;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering
{
    internal class FrameBufferFactory
    {
        private readonly Texture2DFactory _texture2DFactory;
        private readonly ShaderResourceViewFactory _shaderResourceViewFactory;
        private readonly RenderTargetViewFactory _renderTargetViewFactory;
        private readonly DepthStencilViewFactory _depthStencilViewFactory;
        private readonly Swapchain _swapchain;

        public FrameBufferFactory(Texture2DFactory texture2DFactory, ShaderResourceViewFactory shaderResourceViewFactory, RenderTargetViewFactory renderTargetViewFactory, DepthStencilViewFactory depthStencilViewFactory, Swapchain swapchain)
        {
            _texture2DFactory = texture2DFactory;
            _shaderResourceViewFactory = shaderResourceViewFactory;
            _renderTargetViewFactory = renderTargetViewFactory;
            _depthStencilViewFactory = depthStencilViewFactory;
            _swapchain = swapchain; // TODO: is the swapchain the best way to get the width/height of the screen?
        }
        
        internal unsafe FrameBuffer Create(FrameBufferSpecification specification)
        {
            // TODO: support a multiplier, for example
            var width = specification.Width == 0 ? _swapchain.Width : specification.Width;
            var height = specification.Height == 0 ? _swapchain.Height : specification.Height;

            var textures = specification.Textures.Select(t =>
            {
                var format = (DXGI_FORMAT)t.Format;
                if (t.Name == "$Backbuffer")
                {
                    return new FrameBufferTexture(t.Name, null, null, _renderTargetViewFactory.CreateBackbuffer(), format, 0, 0, t.Clear, t.ClearColor);
                }

                var texture2D = _texture2DFactory.Create(width, height, format, D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET);
                var renderTarget = _renderTargetViewFactory.Create(texture2D.AsResource(), format);
                var shaderResource = _shaderResourceViewFactory.Create(texture2D.AsResource(), format);
                return new FrameBufferTexture(t.Name, texture2D, shaderResource, renderTarget, format, height, width, t.Clear, t.ClearColor);
            }).ToArray();

            if (specification.DepthStencil == DepthStencilFormats.None)
            {
                return new FrameBuffer(textures);
            }

            var textureFormat = specification.DepthStencil switch
            {
                DepthStencilFormats.D16 => DXGI_FORMAT.DXGI_FORMAT_R16_TYPELESS,
                DepthStencilFormats.D24S8 => DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS,
                _ => throw new NotSupportedException($"Format: {specification.DepthStencil} is not supported.")
            };

            
            var texture = _texture2DFactory.Create(width, height, textureFormat, bindFlag: D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
            
            var depthStencilFormat = (DXGI_FORMAT)specification.DepthStencil;
            var depthStencilView = _depthStencilViewFactory.Create(texture, depthStencilFormat);
            var depthStencil = new DepthStencil(texture, depthStencilView.AsPointer(), depthStencilFormat, width, height);
            return new FrameBuffer(textures, depthStencil);
        }
    }
}
