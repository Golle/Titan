using System;
using Titan.Graphics.Resources;
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

        public void ResizeBuffers();





        unsafe ID3D11Device* Ptr { get; }
        unsafe ID3D11DeviceContext* ImmediateContextPtr { get; }
        ref readonly ComPtr<ID3D11RenderTargetView> BackBuffer { get; }
        ref readonly ComPtr<IDXGISwapChain> SwapChain { get; }
    }
}
