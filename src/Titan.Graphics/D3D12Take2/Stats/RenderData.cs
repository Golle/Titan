using Titan.Core;

namespace Titan.Graphics.D3D12Take2.Stats;

public struct RenderData : IResource
{
    public ulong FrameCount;
    public uint FrameIndex;

    public GPUMemoryStats LocalGPUStats;
    public GPUMemoryStats NonLocalGPUStats;
}

public struct GPUMemoryStats
{
    public ulong Budget;
    public ulong CurrentUsage;
    public ulong AvailableForReservation;
    public ulong CurrentReservation;
}
