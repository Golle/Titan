using System;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.D3D11.D3D11_FILTER;
using static Titan.Windows.Win32.D3D11.D3D11_TEXTURE_ADDRESS_MODE;
using static Titan.Windows.Win32.D3D11.D3D11_COMPARISON_FUNC;

namespace Titan.Graphics.States
{
    public interface ISamplerStateManager : IDisposable
    {
        SamplerStateHandle GetOrCreate(
            D3D11_FILTER filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR,
            D3D11_TEXTURE_ADDRESS_MODE addressU = D3D11_TEXTURE_ADDRESS_WRAP,
            D3D11_TEXTURE_ADDRESS_MODE addressV = D3D11_TEXTURE_ADDRESS_WRAP,
            D3D11_TEXTURE_ADDRESS_MODE addressW = D3D11_TEXTURE_ADDRESS_WRAP,
            D3D11_COMPARISON_FUNC comparisonFunc = D3D11_COMPARISON_NEVER
        );

        ref readonly SamplerState this[in SamplerStateHandle handle] { get; }
    }
}
