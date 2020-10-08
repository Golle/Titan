using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.D3D11
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_MAPPED_SUBRESOURCE
    {
        public unsafe void* pData;
        public uint RowPitch;
        public uint DepthPitch;
    }
}
