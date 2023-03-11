using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12.Utils;
internal static class CD3DX12_ROOT_PARAMETER1
{
    public static unsafe D3D12_ROOT_PARAMETER1 AsDescriptorTable(uint numberOfDescriptorRanges, D3D12_DESCRIPTOR_RANGE1* descriptorRanges, D3D12_SHADER_VISIBILITY visibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL) =>
        new()
        {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE,
            ShaderVisibility = visibility,
            DescriptorTable = new D3D12_ROOT_DESCRIPTOR_TABLE1
            {
                NumDescriptorRanges = numberOfDescriptorRanges,
                pDescriptorRanges = descriptorRanges
            }
        };

    public static D3D12_ROOT_PARAMETER1 AsConstantBufferView(uint shaderRegister = 0u, uint registerSpace = 0u, D3D12_ROOT_DESCRIPTOR_FLAGS flags = D3D12_ROOT_DESCRIPTOR_FLAGS.D3D12_ROOT_DESCRIPTOR_FLAG_NONE, D3D12_SHADER_VISIBILITY visibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL) =>
        new()
        {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_CBV,

            ShaderVisibility = visibility,
            Descriptor = new()
            {
                RegisterSpace = registerSpace,
                ShaderRegister = shaderRegister,
                Flags = flags
            }
        };

    public static D3D12_ROOT_PARAMETER1 AsShaderResourceView(uint shaderRegister = 0u, uint registerSpace = 0u, D3D12_ROOT_DESCRIPTOR_FLAGS flags = D3D12_ROOT_DESCRIPTOR_FLAGS.D3D12_ROOT_DESCRIPTOR_FLAG_NONE, D3D12_SHADER_VISIBILITY visibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL) =>
        new()
        {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_SRV,

            ShaderVisibility = visibility,
            Descriptor = new()
            {
                RegisterSpace = registerSpace,
                ShaderRegister = shaderRegister,
                Flags = flags
            }
        };
    public static D3D12_ROOT_PARAMETER1 AsUnorderedAccessView(uint shaderRegister = 0u, uint registerSpace = 0u, D3D12_ROOT_DESCRIPTOR_FLAGS flags = D3D12_ROOT_DESCRIPTOR_FLAGS.D3D12_ROOT_DESCRIPTOR_FLAG_NONE, D3D12_SHADER_VISIBILITY visibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL) =>
        new()
        {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_UAV,
            ShaderVisibility = visibility,
            Descriptor = new()
            {
                RegisterSpace = registerSpace,
                ShaderRegister = shaderRegister,
                Flags = flags
            }
        };

    public static D3D12_ROOT_PARAMETER1 AsConstants(uint num32BitValues, uint shaderRegister = 0u, uint registerSpace = 0u, D3D12_SHADER_VISIBILITY visibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL) =>
        new()
        {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_32BIT_CONSTANTS,
            Constants = new D3D12_ROOT_CONSTANTS
            {
                Num32BitValues = num32BitValues,
                RegisterSpace = registerSpace,
                ShaderRegister = shaderRegister
            },
            ShaderVisibility = visibility
        };
}
