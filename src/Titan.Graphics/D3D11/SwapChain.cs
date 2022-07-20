using System;
using System.Runtime.CompilerServices;
using Titan.Windows;
using Titan.Windows.DXGI;

namespace Titan.Graphics.D3D11
{
    public unsafe class SwapChain : IDisposable
    {
        public bool Vsync { get; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        private ComPtr<IDXGISwapChain> _swapChain;
        private readonly uint _syncInterval;
        public SwapChain(IDXGISwapChain* swapChain, DeviceConfiguration config)
        {
            Vsync = config.Vsync;
            Width = config.Width;
            Height = config.Height;
            _swapChain = new ComPtr<IDXGISwapChain>(swapChain);
            _syncInterval = Vsync ? 1u : 0u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Resize(uint width, uint height)
        {
            if (height != 0)
            {
                Height = height;
            }
            if (width != 0)
            {
                Width = width;
            }
            _swapChain.Get()->ResizeBuffers(0, width, height, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Present() => _swapChain.Get()->Present(_syncInterval, 0);

        public void Dispose()
        {
            _swapChain.Dispose();
        }
    }
}
