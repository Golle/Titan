using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11_BLEND;
using static Titan.Windows.Win32.D3D11.D3D11_BLEND_OP;
using static Titan.Windows.Win32.D3D11.D3D11_COLOR_WRITE_ENABLE;

namespace Titan.Graphics.D3D11.State
{
    //TODO: this should be handled in a manager
    public unsafe class BlendState : IDisposable
    {
        internal ref readonly ComPtr<ID3D11BlendState> Ptr => ref _blendState;

        private ComPtr<ID3D11BlendState> _blendState;
        internal uint SampleMask { get; }
        internal float* BlendFactor => (float*) Unsafe.AsPointer(ref _blendFactor);
        private Color _blendFactor;


        public BlendState(IGraphicsDevice graphicsDevice)
        {
            D3D11_BLEND_DESC desc = default;
            for (var i = 0; i < 8; ++i)
            {
                ref var renderTarget = ref desc.RenderTarget[i];
                renderTarget.BlendEnable = false;
                renderTarget.BlendOp = D3D11_BLEND_OP_ADD;
                renderTarget.RenderTargetWriteMask = (byte)D3D11_COLOR_WRITE_ENABLE_ALL;
                renderTarget.SrcBlend = D3D11_BLEND_ONE;
                renderTarget.DestBlend = D3D11_BLEND_ZERO;
                renderTarget.SrcBlendAlpha = D3D11_BLEND_ONE;
                renderTarget.DestBlendAlpha = D3D11_BLEND_ZERO;
                renderTarget.BlendOpAlpha = D3D11_BLEND_OP_ADD;
            }

            desc.RenderTarget[0].BlendEnable = true;
            desc.RenderTarget[0].RenderTargetWriteMask = (byte)D3D11_COLOR_WRITE_ENABLE_ALL;
            desc.RenderTarget[0].SrcBlend = D3D11_BLEND_INV_SRC_ALPHA;
            desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;

            desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_INV_DEST_ALPHA;
            desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ONE;
            desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;


            if (graphicsDevice is D3D11GraphicsDevice device)
            {
                CheckAndThrow(device.Ptr->CreateBlendState(&desc, _blendState.GetAddressOf()), "CreateBlendState");
            }
            else
            {
                throw new ArgumentException($"Trying to initialize a D3D11 {nameof(BlendState)} with the wrong device.", nameof(graphicsDevice));
            }

            _blendFactor = new Color(1f, 1f, 1f, 1f);
            SampleMask = 0xffffffff;
        }

        public void Dispose() => _blendState.Dispose();
    }
}
