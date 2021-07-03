using System.Runtime.InteropServices;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Buffer
    {
        // D3d pointer
        public ID3D11Buffer* Resource;

        public uint Count;
        public uint Stride;

        public int Handle;

        // d3d specific properties
        public D3D11_USAGE Usage;
        public D3D11_CPU_ACCESS_FLAG CpuAccessFlag;
        public D3D11_RESOURCE_MISC_FLAG MiscFlag;
        public D3D11_BIND_FLAG BindFlag;
    }
}
