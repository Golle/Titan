using Titan.Core;

namespace Titan.Graphics.D3D12;

public struct GraphicsDevice : IApi
{
    public D3D12Device Device;
}


public struct RenderContext : IApi
{
    public D3D12RenderContext Context;
}
