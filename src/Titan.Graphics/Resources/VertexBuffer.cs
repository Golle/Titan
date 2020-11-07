using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct VertexBuffer
    {
        [FieldOffset(0)]
        public ID3D11Buffer* Pointer;
        [FieldOffset(0)]
        public ID3D11Resource* Resource;
        [FieldOffset(8)]
        public uint Stride;
        [FieldOffset(8 + sizeof(uint))]
        public D3D11_USAGE Usage;
        [FieldOffset(8 + sizeof(uint) + sizeof(D3D11_USAGE))]
        public D3D11_CPU_ACCESS_FLAG CpuAccessFlag;
    }
}
