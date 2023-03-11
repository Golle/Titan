namespace Titan.Graphics.Resources;

public readonly struct GPUBuffer
{
    public readonly BufferType Type;
    internal GPUBuffer(BufferType type)
    {
        Type = type;
    }
}
