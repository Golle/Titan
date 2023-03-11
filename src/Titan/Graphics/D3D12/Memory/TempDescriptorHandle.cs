using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12.Memory;

[SkipLocalsInit]
[DebuggerDisplay("CPU: {CPU.ptr, nq} GPU: {GPU.ptr, nq} Index: {Index, nq}")]
internal readonly struct TempDescriptorHandle
{
    public readonly D3D12_CPU_DESCRIPTOR_HANDLE CPU;
    public readonly D3D12_GPU_DESCRIPTOR_HANDLE GPU;
    public readonly uint Index;
    public TempDescriptorHandle(uint index, D3D12_CPU_DESCRIPTOR_HANDLE cpu, D3D12_GPU_DESCRIPTOR_HANDLE gpu)
    {
        CPU = cpu;
        GPU = gpu;
        Index = index;
    }
}
