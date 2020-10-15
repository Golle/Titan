using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11_FILTER;
using static Titan.Windows.Win32.D3D11.D3D11_TEXTURE_ADDRESS_MODE;
using static Titan.Windows.Win32.D3D11.D3D11_COMPARISON_FUNC;

namespace Titan.Graphics.D3D11.State
{
    public unsafe class SamplerState : IDisposable
    {
        internal ref readonly ComPtr<ID3D11SamplerState> Ptr => ref _samplerState;
        private ComPtr<ID3D11SamplerState> _samplerState;
        public SamplerState(IGraphicsDevice device)
        {
            D3D11_SAMPLER_DESC samplerDesc;
            samplerDesc.Filter = D3D11_FILTER_COMPARISON_MIN_MAG_MIP_LINEAR;
            samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
            samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
            samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
            samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;

            CheckAndThrow(device.Ptr->CreateSamplerState(&samplerDesc, _samplerState.GetAddressOf()), "CreateSamplerState");
        }
        public void Dispose() => _samplerState.Dispose();
    }
}
