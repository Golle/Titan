namespace Titan.Platform.DXC;

public unsafe struct DXCBuffer
{
    public void* Ptr;
    public nuint Size;
    public uint Encoding;
}
