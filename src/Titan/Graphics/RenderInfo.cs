using Titan.Core.Maths;
using Titan.ECS.Components;
using Titan.Graphics.D3D12;

namespace Titan.Graphics;

internal struct RenderInfo : IResource
{
    public ulong FrameCount;
    public uint BackbufferIndex;
    public Size WindowSize;
    public D3D12Texture CurrentBackbuffer;
    public Color ClearColor;
}
