using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.Resources;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;

namespace Titan.Graphics.D3D12;

internal struct D3D12Buffer
{
    public GPUBuffer Buffer;
    public ComPtr<ID3D12Resource> Resource;
    public DescriptorHandle SRV;

    public uint Size;
    public DXGI_FORMAT Format;
    public D3D12_GPU_VIRTUAL_ADDRESS GPUAddress;
    public void Destroy()
    {
        Resource.Dispose();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_INDEX_BUFFER_VIEW IndexBufferView()
    {
        Debug.Assert(Format is DXGI_FORMAT.DXGI_FORMAT_R16_UINT or DXGI_FORMAT.DXGI_FORMAT_R32_UINT);
        Unsafe.SkipInit(out D3D12_INDEX_BUFFER_VIEW view);
        view.Format = Format;
        view.BufferLocation = GPUAddress;
        view.SizeInBytes = Size;
        return view;
    }
}
