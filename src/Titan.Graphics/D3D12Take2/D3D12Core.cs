using Titan.Core;

namespace Titan.Graphics.D3D12Take2;

internal struct D3D12Core : IResource
{
    public D3D12Device Device;
    public D3D12Surface Surface;
    public D3D12Command Command;
    public D3D12GraphicsQueue Queue;
}
