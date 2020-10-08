// ReSharper disable InconsistentNaming
namespace Titan.D3D11
{
    public unsafe struct D3D11_INPUT_ELEMENT_DESC
    {
        public char* SemanticName;
        public uint SemanticIndex;
        public DXGI_FORMAT Format;
        public uint InputSlot;
        public uint AlignedByteOffset;
        public D3D11_INPUT_CLASSIFICATION InputSlotClass;
        public uint InstanceDataStepRate;
    }
}
