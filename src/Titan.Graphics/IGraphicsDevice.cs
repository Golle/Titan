using System;
using Titan.Graphics.D3D11;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics
{
    public interface IGraphicsDevice : IDisposable
    {
        IShaderResourceViewManager ShaderResourceViewManager { get; }
        ITextureManager TextureManager { get; }
        IVertexBufferManager VertexBufferManager { get; }
        IIndexBufferManager IndexBufferManager { get; }
        IConstantBufferManager ConstantBufferManager { get; }
        IRenderTargetViewManager RenderTargetViewManager{ get; }
        IDepthStencilViewManager DepthStencilViewManager { get; }
        IDepthStencilStateManager DepthStencilStateManager { get; }
        ISamplerStateManager SamplerStateManager { get; }
        IShaderManager ShaderManager { get; }
        IRenderContext ImmediateContext { get; }


        unsafe ID3D11Device* Ptr { get; }
        ref readonly ComPtr<IDXGISwapChain> SwapChain { get; }

        public void ResizeBuffers();
    }
}
