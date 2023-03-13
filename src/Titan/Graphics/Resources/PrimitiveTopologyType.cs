using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.Resources;

public enum PrimitiveTopologyType
{
    Unknown,
    Point = D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_POINT,
    Line = D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_LINE,
    Triangle = D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE
}