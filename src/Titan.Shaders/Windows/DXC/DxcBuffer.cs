namespace Titan.Shaders.Windows.DXC;

public unsafe struct DxcBuffer
{
    public void* Ptr;
    public nuint Size;
    public uint Encoding;
}