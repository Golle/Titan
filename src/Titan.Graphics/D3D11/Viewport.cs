using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Viewport
    {
        private readonly D3D11_VIEWPORT _viewport;
        public Viewport(float width, float height, float topLeftX = 0f, float topLeftY = 0f, float minDepth = 0f, float maxDepth = 1f)
        {
            _viewport.Width = width;
            _viewport.Height = height;
            _viewport.TopLeftX = topLeftX;
            _viewport.TopLeftY = topLeftY;
            _viewport.MinDepth = minDepth;
            _viewport.MaxDepth = maxDepth;
        }
    }
}
