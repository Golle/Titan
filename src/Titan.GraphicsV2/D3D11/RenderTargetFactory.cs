using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    public unsafe class RenderTargetFactory
    {
        private readonly Device _device;
        private readonly Swapchain _swapchain;

        internal RenderTargetFactory(Device device, Swapchain swapchain)
        {
            _device = device;
            _swapchain = swapchain;
        }

        internal RenderTargetView CreateBackbuffer()
        {
            using var backbufferResource = new ComPtr<ID3D11Resource>();
            ID3D11RenderTargetView* backbuffer;
            fixed (Guid* resourcePointer = &D3D11Common.D3D11Resource)
            {
                Common.CheckAndThrow(_swapchain.Get()->GetBuffer(0, resourcePointer, (void**)backbufferResource.GetAddressOf()), nameof(IDXGISwapChain.GetBuffer));
            }
            Common.CheckAndThrow(_device.Get()->CreateRenderTargetView(backbufferResource.Get(), null, &backbuffer), nameof(ID3D11Device.CreateRenderTargetView));
            return new RenderTargetView {View = backbuffer};
        }

        internal object Create(in Texture2D resource)
        {
            return null;
        }
    }
}