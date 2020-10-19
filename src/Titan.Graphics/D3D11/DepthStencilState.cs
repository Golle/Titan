using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class DepthStencilState : IDisposable
    {
        internal ref readonly ComPtr<ID3D11DepthStencilState> Ptr => ref _depthStencilState;
        private ComPtr<ID3D11DepthStencilState> _depthStencilState;

        public DepthStencilState(IGraphicsDevice device)
        {
            D3D11_DEPTH_STENCIL_DESC desc = default;
            desc.DepthEnable = 1;
            //depthDesc.DepthWriteMask = D3D11DepthWriteMask.Zero;
            //depthDesc.DepthFunc = D3D11ComparisonFunc.LessEqual;  // this is needed to add alpha-blending to sprites, not sure why.
            desc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL;
            desc.DepthFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS_EQUAL;  // this is needed to add alpha-blending to sprites, not sure why.
            CheckAndThrow(device.Ptr->CreateDepthStencilState(&desc, _depthStencilState.GetAddressOf()), "CreateDepthStencilState");
        }

        public void Dispose() => _depthStencilState.Dispose();
    }
}
