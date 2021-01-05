using System;
using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IRenderTargetViewManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        unsafe Handle<RenderTargetView> Create(ID3D11Resource* resource, DXGI_FORMAT format);
        void Destroy(in Handle<RenderTargetView> handle);
        ref readonly RenderTargetView this[in Handle<RenderTargetView> handle] { get; }
        Handle<RenderTargetView> BackbufferHandle { get; }
    }
}
