namespace Titan.Platform.Win32.D3D12;

public struct D3D12_GPU_VIRTUAL_ADDRESS
{
    public ulong Address;
    public D3D12_GPU_VIRTUAL_ADDRESS(ulong address) => Address = address;
    public static implicit operator ulong(in D3D12_GPU_VIRTUAL_ADDRESS a) => a.Address;
    public static implicit operator D3D12_GPU_VIRTUAL_ADDRESS(ulong address) => new(address);
}