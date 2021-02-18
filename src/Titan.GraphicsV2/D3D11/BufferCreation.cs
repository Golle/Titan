using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe struct BufferCreation
    {
        internal D3D11_USAGE Usage;
        internal D3D11_CPU_ACCESS_FLAG CpuAccessFlags;
        internal D3D11_RESOURCE_MISC_FLAG MiscFlags;

        internal BufferTypes Type;
        internal uint Count;
        internal uint Stride;

        internal void* InitialData;
    }
}
