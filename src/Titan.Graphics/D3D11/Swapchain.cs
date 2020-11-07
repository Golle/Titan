using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class Swapchain : IDisposable
    {
        private ComPtr<IDXGISwapChain> _swapChain;

        private readonly uint _syncInterval;
        private readonly uint _flags;
        public Swapchain(IGraphicsDevice device, bool vsync, uint flags = 0u)
        {
            _flags = flags;
            _syncInterval = vsync ? 1 : 0;
            _swapChain = new ComPtr<IDXGISwapChain>(device.SwapChain.Get());
        }

        public DXGI_SWAP_CHAIN_DESC GetDesc()
        {
            DXGI_SWAP_CHAIN_DESC desc;
            CheckAndThrow(_swapChain.Get()->GetDesc(&desc), "IDXGISwapChain::GetDesc");
            return desc;
        }



        public void ResizeTarget(uint width, uint height)
        {
            var swapChainDesc = GetDesc();
            var bufferDesc = swapChainDesc.BufferDesc;
            bufferDesc.Width = width;
            bufferDesc.Height = height;
            
            CheckAndThrow(_swapChain.Get()->ResizeTarget(&bufferDesc), "IDXGISwapChain::ResizeTarget");
        }

        public void ResizeBuffers()
        {
            CheckAndThrow(_swapChain.Get()->ResizeBuffers(0, 0, 0, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0), "IDXGISwapChain::ResizeTarget");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Present() => _swapChain.Get()->Present(_syncInterval, _flags);

        public void Dispose() => _swapChain.Dispose();
    }
}