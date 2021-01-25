using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11.RenderTarget
{
    internal readonly unsafe struct RenderTargetView
    {
        internal readonly ID3D11RenderTargetView* View;
        internal readonly ID3D11Resource* Resource;
        public RenderTargetView(ID3D11RenderTargetView* renderTargetView, ID3D11Resource* resource)
        {
            View = renderTargetView;
            Resource = resource;
        }
    }
}
