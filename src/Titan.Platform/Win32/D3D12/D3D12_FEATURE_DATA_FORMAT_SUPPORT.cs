using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

[StructLayout(LayoutKind.Sequential)]
public struct D3D12_FEATURE_DATA_FORMAT_SUPPORT
{
    public DXGI_FORMAT Format;
    public D3D12_FORMAT_SUPPORT1 Support1;
    public D3D12_FORMAT_SUPPORT2 Support2;
}
