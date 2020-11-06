using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public unsafe struct IndexBuffer
    {
        public ID3D11Buffer* Raw;
        public DXGI_FORMAT Format;
        public uint NumberOfIndices;
    }
}
