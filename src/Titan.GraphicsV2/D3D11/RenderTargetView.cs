using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    public readonly unsafe struct RenderTargetView
    {
        public readonly ID3D11RenderTargetView* View;
        public RenderTargetView(ID3D11RenderTargetView* view)
        {
            View = view;
        }
    }
}
