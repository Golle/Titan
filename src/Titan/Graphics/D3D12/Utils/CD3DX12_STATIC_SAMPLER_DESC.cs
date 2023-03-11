using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12.Utils;

internal static class CD3DX12_STATIC_SAMPLER_DESC
{
    public static D3D12_STATIC_SAMPLER_DESC Init(
        uint shaderRegister = 0u,
        uint registerSpace = 0u,
        D3D12_FILTER filter = D3D12_FILTER.D3D12_FILTER_ANISOTROPIC,
        D3D12_TEXTURE_ADDRESS_MODE addressU = D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP,
        D3D12_TEXTURE_ADDRESS_MODE addressV = D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP,
        D3D12_TEXTURE_ADDRESS_MODE addressW = D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP,
        float mipLODBias = 0,
        uint maxAnisotropy = 16,
        D3D12_COMPARISON_FUNC comparisonFunc = D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_LESS_EQUAL,
        D3D12_STATIC_BORDER_COLOR borderColor = D3D12_STATIC_BORDER_COLOR.D3D12_STATIC_BORDER_COLOR_OPAQUE_WHITE,
        float minLOD = 0,
        float maxLOD = D3D12Constants.D3D12_FLOAT32_MAX,
        D3D12_SHADER_VISIBILITY shaderVisibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL
    ) =>
        new()
        {
            ShaderVisibility = shaderVisibility,
            RegisterSpace = registerSpace,
            ShaderRegister = shaderRegister,
            AddressU = addressU,
            AddressV = addressV,
            AddressW = addressW,
            BorderColor = borderColor,
            ComparisonFunc = comparisonFunc,
            Filter = filter,
            MaxAnisotropy = maxAnisotropy,
            MaxLOD = maxLOD,
            MinLOD = minLOD,
            MipLODBias = mipLODBias
        };
}
