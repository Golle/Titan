using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_CLEAR_VALUE
{
    public DXGI_FORMAT Format;

    private D3D12_CLEAR_VALUE_UNION _union;
    public float* Color => ((D3D12_CLEAR_VALUE_UNION*)Unsafe.AsPointer(ref _union))->Color;
    public ref D3D12_DEPTH_STENCIL_VALUE DepthStencil => ref ((D3D12_CLEAR_VALUE_UNION*)Unsafe.AsPointer(ref _union))->DepthStencil;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_CLEAR_VALUE_UNION
    {
        [FieldOffset(0)]
        public fixed float Color[4];
        [FieldOffset(0)]
        public D3D12_DEPTH_STENCIL_VALUE DepthStencil;
    }
}
