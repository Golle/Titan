using System;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics
{
    public interface IGraphicsDevice : IDisposable
    {
        void Initialize(uint refreshRate, bool debug = false);
        IRenderContext ImmediateContext { get; }

        unsafe ID3D11Device* Ptr { get; }
        ref readonly ComPtr<IDXGISwapChain> SwapChain { get; }

        public void ResizeBuffers();
    }
}
