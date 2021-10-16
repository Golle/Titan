// ReSharper disable InconsistentNaming

using Titan.Windows.DXGI;

namespace Titan.Windows.D3D11
{
    public unsafe struct D3D11_INPUT_ELEMENT_DESC
    {
        public sbyte* SemanticName;
        public uint SemanticIndex;
        public DXGI_FORMAT Format;
        public uint InputSlot;
        public uint AlignedByteOffset;
        public D3D11_INPUT_CLASSIFICATION InputSlotClass;
        public uint InstanceDataStepRate;
    }
}
