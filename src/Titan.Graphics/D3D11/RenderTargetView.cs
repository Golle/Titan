using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class RenderTargetView : IDisposable
    {
        private ComPtr<ID3D11RenderTargetView> _renderTargetView;
        internal ref readonly ComPtr<ID3D11RenderTargetView> Ptr => ref _renderTargetView;

        public RenderTargetView(IGraphicsDevice device)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        internal RenderTargetView(ID3D11RenderTargetView* renderTargetView)
        {
            _renderTargetView = new ComPtr<ID3D11RenderTargetView>(renderTargetView);
        }

        public void Dispose() => _renderTargetView.Dispose();
    }
}
