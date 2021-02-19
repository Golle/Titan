using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Buffers
{
    internal record BufferCreation 
    {
        internal D3D11_USAGE Usage { get; init; }
        internal D3D11_CPU_ACCESS_FLAG CpuAccessFlags { get; init; }
        internal D3D11_RESOURCE_MISC_FLAG MiscFlags { get; init; }

        internal BufferTypes Type { get; init; }
        internal uint Count { get; init; }
        internal uint Stride { get; init; }

        internal DataBlob InitialData { get; init; }
    }
}
