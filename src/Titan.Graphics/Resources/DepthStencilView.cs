using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public unsafe struct DepthStencilView
    {
        public ID3D11DepthStencilView* Pointer;
        public DXGI_FORMAT Format;
    }
}
