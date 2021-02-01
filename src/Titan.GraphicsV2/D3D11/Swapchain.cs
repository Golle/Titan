using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Swapchain : IDisposable
    {
        private ComPtr<IDXGISwapChain> _swapChain;
        private readonly uint _syncInterval;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDXGISwapChain* Get() => _swapChain.Get();
        internal Swapchain(IDXGISwapChain* swapChain, bool vsync)
        {
            _swapChain = new ComPtr<IDXGISwapChain>(swapChain);
            _syncInterval = vsync ? 1u : 0u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Present() => _swapChain.Get()->Present(_syncInterval, 0);

        public void Dispose() => _swapChain.Dispose();
    }
}
