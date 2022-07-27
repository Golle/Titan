using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.D3D12;

public struct D3D12_ROOT_PARAMETER1
{
    public D3D12_ROOT_PARAMETER_TYPE ParameterType;
    private D3D12_ROOT_PARAMETER1_UNION UnionMembers;
    public unsafe ref D3D12_ROOT_DESCRIPTOR_TABLE1 DescriptorTable => ref ((D3D12_ROOT_PARAMETER1_UNION*)Unsafe.AsPointer(ref UnionMembers))->DescriptorTable;
    public unsafe ref D3D12_ROOT_CONSTANTS Constants => ref ((D3D12_ROOT_PARAMETER1_UNION*)Unsafe.AsPointer(ref UnionMembers))->Constants;
    public unsafe ref D3D12_ROOT_DESCRIPTOR1 Descriptor => ref ((D3D12_ROOT_PARAMETER1_UNION*)Unsafe.AsPointer(ref UnionMembers))->Descriptor;
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