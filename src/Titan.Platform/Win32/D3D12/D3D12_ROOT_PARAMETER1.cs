using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_ROOT_PARAMETER1
{
    public D3D12_ROOT_PARAMETER_TYPE ParameterType;
    private D3D12_ROOT_PARAMETER1_UNION _union;

    [UnscopedRef]
    public ref D3D12_ROOT_DESCRIPTOR_TABLE1 DescriptorTable => ref _union.DescriptorTable;
    [UnscopedRef]
    public ref D3D12_ROOT_CONSTANTS Constants => ref _union.Constants;
    [UnscopedRef]
    public ref D3D12_ROOT_DESCRIPTOR1 Descriptor => ref _union.Descriptor;

    public D3D12_SHADER_VISIBILITY ShaderVisibility;
    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_ROOT_PARAMETER1_UNION
    {
        [FieldOffset(0)]
        public D3D12_ROOT_DESCRIPTOR_TABLE1 DescriptorTable;
        [FieldOffset(0)]
        public D3D12_ROOT_CONSTANTS Constants;
        [FieldOffset(0)]
        public D3D12_ROOT_DESCRIPTOR1 Descriptor;
    }
}
