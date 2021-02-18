using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe struct Buffer
    {
        // D3d pointer
        internal ID3D11Buffer* Resource;

        internal uint Count;
        internal uint Stride;

        internal int Handle;
        
        // d3d specific properties
        internal D3D11_USAGE Usage;
        internal D3D11_CPU_ACCESS_FLAG CpuAccessFlag;
        internal D3D11_RESOURCE_MISC_FLAG MiscFlag;
        internal D3D11_BIND_FLAG BindFlag;
    }
}
