using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public interface IGraphicsDevice : IDisposable
    {
        internal unsafe ID3D11Device* Ptr { get; }
        internal unsafe ID3D11DeviceContext* ImmediateContextPtr { get; }
        internal ref readonly ComPtr<ID3D11RenderTargetView> BackBuffer { get; }
        internal ref readonly ComPtr<IDXGISwapChain> SwapChain { get; }
    }
}
