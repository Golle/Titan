using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12.Memory;

[DebuggerDisplay("CPU: {CPU.ptr, nq} GPU: {GPU.ptr, nq}")]
internal readonly struct DescriptorHandle
{
    public readonly D3D12_CPU_DESCRIPTOR_HANDLE CPU;
    public readonly D3D12_GPU_DESCRIPTOR_HANDLE GPU;
    public readonly ulong Index;
    public readonly uint Offset;
    public readonly DescriptorHeapType Type;

    public DescriptorHandle(DescriptorHeapType type, uint offset, D3D12_CPU_DESCRIPTOR_HANDLE cpu, D3D12_GPU_DESCRIPTOR_HANDLE gpu, ulong index)
    {
        Type = type;
        Offset = offset;
        CPU = cpu;
        GPU = gpu;
        Index = index;
    }

    public bool IsShaderVisible => GPU.ptr != 0;
    public bool IsValid => Offset != 0;
    public static readonly DescriptorHandle Invalid = default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator D3D12_CPU_DESCRIPTOR_HANDLE(in DescriptorHandle handle) => handle.CPU;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator D3D12_GPU_DESCRIPTOR_HANDLE(in DescriptorHandle handle) => handle.GPU;
}
