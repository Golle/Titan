using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_VERSIONED_ROOT_SIGNATURE_DESC
{
    public D3D_ROOT_SIGNATURE_VERSION Version;
    private Union _union;
    [UnscopedRef]
    public ref D3D12_ROOT_SIGNATURE_DESC Desc_1_0 => ref _union.Desc_1_0;
    [UnscopedRef]
    public ref D3D12_ROOT_SIGNATURE_DESC1 Desc_1_1 => ref _union.Desc_1_1;

    [StructLayout(LayoutKind.Explicit)]
    private struct Union
    {
        [FieldOffset(0)]
        public D3D12_ROOT_SIGNATURE_DESC Desc_1_0;
        [FieldOffset(0)]
        public D3D12_ROOT_SIGNATURE_DESC1 Desc_1_1;
    }
}
