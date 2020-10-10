using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;
using static Titan.Windows.Win32.D3D11.D3D11_FILTER;
using static Titan.Windows.Win32.D3D11.D3D11_TEXTURE_ADDRESS_MODE;

namespace Titan.Graphics.D3D11
{
    public unsafe class Sampler : IDisposable
    {
        private ComPtr<ID3D11SamplerState> _samplerState;
        public Sampler(IGraphicsDevice device)
        {
            D3D11_SAMPLER_DESC samplerDesc;
            samplerDesc.Filter = D3D11_FILTER_COMPARISON_MIN_LINEAR_MAG_MIP_POINT;
            samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
            samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
            samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;

            CheckAndThrow(device.Ptr->CreateSamplerState(&samplerDesc, _samplerState.GetAddressOf()), "CreateSamplerState");
        }

        public void Dispose()
        {
            _samplerState.Dispose();
        }

    }
}
