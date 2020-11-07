using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct IndexBuffer
    {
        [FieldOffset(0)]
        public ID3D11Buffer* Pointer;
        [FieldOffset(0)]
        public ID3D11Resource* Resource;
        [FieldOffset(8)]
        public DXGI_FORMAT Format;
        [FieldOffset(8 + sizeof(DXGI_FORMAT))]
        public uint NumberOfIndices;
    }
}
