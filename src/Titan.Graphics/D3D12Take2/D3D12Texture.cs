using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;

namespace Titan.Graphics.D3D12Take2;

public struct D3D12Texture
{
    public ComPtr<ID3D12Resource> Resource;
    public D3D12_RESOURCE_STATES State;
    public DXGI_FORMAT Format;
    public uint Width;
    public uint Height;

    public DescriptorHandle RTV; // might need separate ones for CBV, SRV, UAV and DSV if we're going to use this for everything

    //public uint NumMips;
    //public uint NumElements;
    //public uint RowPitch;
    //public uint SlicePitch;

    //public D3D12_CPU_DESCRIPTOR_HANDLE CBV;
    //public D3D12_CPU_DESCRIPTOR_HANDLE SRV;
    //public D3D12_CPU_DESCRIPTOR_HANDLE UAV;
    //public D3D12_CPU_DESCRIPTOR_HANDLE RTV;
    //public D3D12_CPU_DESCRIPTOR_HANDLE DSV;
}
