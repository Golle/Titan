using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Buffers
{
    public readonly record struct BufferCreation
    {
        public D3D11_USAGE Usage { get; init; }
        public D3D11_CPU_ACCESS_FLAG CpuAccessFlags { get; init; }
        public D3D11_RESOURCE_MISC_FLAG MiscFlags { get; init; }

        public BufferTypes Type { get; init; }
        public uint Count { get; init; }
        public uint Stride { get; init; }

        public DataBlob InitialData { get; init; }
    }
}
