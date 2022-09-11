namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_SO_DECLARATION_ENTRY
{
    public uint Stream;
    public byte* SemanticName;
    public uint SemanticIndex;
    public byte StartComponent;
    public byte ComponentCount;
    public byte OutputSlot;
}