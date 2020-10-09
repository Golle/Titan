// ReSharper disable InconsistentNaming

namespace Titan.D3D11
{
    public struct D3D11_BUFFER_DESC
    {
        public uint ByteWidth;
        public D3D11_USAGE Usage;
        public D3D11_BIND_FLAG BindFlags;
        public D3D11_CPU_ACCESS_FLAG CpuAccessFlags;
        public D3D11_RESOURCE_MISC_FLAG MiscFlags;
        public uint StructureByteStride;
    }
}
