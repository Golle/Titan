using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11_BLEND;
using static Titan.Windows.Win32.D3D11.D3D11_BLEND_OP;
using static Titan.Windows.Win32.D3D11.D3D11_COLOR_WRITE_ENABLE;

namespace Titan.Graphics.D3D11.State
{
    public unsafe class BlendState : IDisposable
    {
        internal ref readonly ComPtr<ID3D11BlendState> Ptr => ref _blendState;

        private ComPtr<ID3D11BlendState> _blendState;

        public BlendState(IGraphicsDevice device)
        {
            D3D11_BLEND_DESC desc = default;
            ref var renderTarget = ref desc.RenderTarget[0];
            renderTarget.BlendEnable = true;
            renderTarget.RenderTargetWriteMask = (byte)D3D11_COLOR_WRITE_ENABLE_ALL;
            renderTarget.SrcBlend = D3D11_BLEND_SRC_ALPHA;
            renderTarget.DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
            renderTarget.SrcBlendAlpha = D3D11_BLEND_INV_DEST_ALPHA;
            renderTarget.DestBlendAlpha = D3D11_BLEND_ONE;
            renderTarget.BlendOpAlpha = D3D11_BLEND_OP_ADD;
            
            CheckAndThrow(device.Ptr->CreateBlendState(&desc, _blendState.GetAddressOf()), "CreateBlendState");
        }

        public void Dispose()
        {
            _blendState.Dispose();
        }
    }
}
