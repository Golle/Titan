using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IRenderTargetViewManager : IDisposable
    {
        unsafe RenderTargetViewHandle Create(ID3D11Resource* resource, DXGI_FORMAT format);
        void Destroy(in RenderTargetViewHandle handle);
        ref readonly RenderTargetView this[in RenderTargetViewHandle handle] { get; }
        RenderTargetViewHandle BackbufferHandle { get; }
    }
}
