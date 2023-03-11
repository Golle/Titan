namespace Titan.Assets;

public struct ImageAssetDescriptor
{
    public uint Width;
    public uint Height;
    public uint Format; //DXGI_FORMAT in current implementation
    public uint Stride;
}
