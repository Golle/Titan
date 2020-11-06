using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    internal unsafe struct VertexBuffer
    {
        public ID3D11Buffer* Raw;
        public D3D11_USAGE Usage;
        public D3D11_CPU_ACCESS_FLAG CpuAccessFlag;
    }
}
