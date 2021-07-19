using System.Runtime.CompilerServices;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class SwapChain
    {
        public bool Vsync { get; }
        public uint Width { get; }
        public uint Height { get; }
        public ID3D11RenderTargetView* Backbuffer { get; }

        private readonly IDXGISwapChain* _swapChain;
        private readonly uint _syncInterval;

        internal SwapChain(IDXGISwapChain* swapChain, ID3D11RenderTargetView* backbuffer, bool vsync, uint width, uint height)
        {
            Vsync = vsync;
            Width = width;
            Height = height;
            Backbuffer = backbuffer;
            _swapChain = swapChain;
            _syncInterval = vsync ? 1u : 0u;
        }
     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Present() => _swapChain->Present(_syncInterval, 0);
    }
}
