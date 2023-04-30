using Titan.Graphics.Resources;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12;

internal unsafe struct D3D12Model3D
{
    public Model3D Model;
    public ID3D12Resource* Resource;
}
