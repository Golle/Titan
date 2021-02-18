using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Swapchain
    {
        public bool Vsync { get; }
        public uint Width { get; }
        public uint Height { get; }

        private readonly IDXGISwapChain* _swapChain;
        private readonly uint _syncInterval;

        internal Swapchain(IDXGISwapChain* swapChain, bool vsync, uint width, uint height)
        {
            Vsync = vsync;
            Width = width;
            Height = height;
            _swapChain = swapChain;
            _syncInterval = vsync ? 1u : 0u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Present() => _swapChain->Present(_syncInterval, 0);

    }
}
