using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    public readonly unsafe struct RenderTargetView
    {
        private readonly ID3D11RenderTargetView* _view;
        public RenderTargetView(ID3D11RenderTargetView* view)
        {
            _view = view;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11RenderTargetView*(in RenderTargetView view) => view._view;
        public void Release() => _view->Release();
    }
}
