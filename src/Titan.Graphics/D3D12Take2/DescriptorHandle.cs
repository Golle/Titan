using System.Runtime.CompilerServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12Take2;

public struct DescriptorHandle
{
    public D3D12_CPU_DESCRIPTOR_HANDLE CpuHandle;
    public D3D12_GPU_DESCRIPTOR_HANDLE GpuHandle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid() => CpuHandle.ptr != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsShaderVisible() => GpuHandle.ptr != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator D3D12_CPU_DESCRIPTOR_HANDLE(in DescriptorHandle handle) => handle.CpuHandle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator D3D12_GPU_DESCRIPTOR_HANDLE(in DescriptorHandle handle) => handle.GpuHandle;
}
