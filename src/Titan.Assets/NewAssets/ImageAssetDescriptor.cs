using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Assets.NewAssets;


public enum AssetDescriptorType
{
    Texture,
    Model,
    Shader,

    Count // Must be the last value
}

public interface IManifestDescriptor
{
    static abstract uint Id { get; }
    static abstract string ManifestFile { get; }
    static abstract string TitanPackageFile { get; }
    static abstract uint AssetCount { get; }
    static abstract AssetDescriptor[] AssetDescriptors { get; }
}
public unsafe struct AssetDescriptor
{
    public uint Id;
    public uint ManifestId;
    public AssetDescriptorType Type;
    public AssetReference Reference;
    private AssetDescriptorUnion _union;
    public ref ImageAssetDescriptor Image => ref ((AssetDescriptorUnion*)Unsafe.AsPointer(ref _union))->ImageAssetDescriptor;
    public ref ModelAssetDescriptor Model => ref ((AssetDescriptorUnion*)Unsafe.AsPointer(ref _union))->ModelAssetDescriptor;
    public ref ShaderAssetDescriptor Shader => ref ((AssetDescriptorUnion*)Unsafe.AsPointer(ref _union))->ShaderAssetDescriptor;

    [StructLayout(LayoutKind.Explicit)]
    private struct AssetDescriptorUnion
    {
        [FieldOffset(0)]
        public ImageAssetDescriptor ImageAssetDescriptor;
        [FieldOffset(0)]
        public ModelAssetDescriptor ModelAssetDescriptor;
        [FieldOffset(0)]
        public ShaderAssetDescriptor ShaderAssetDescriptor;
    }

    public ulong GetSize() => Reference.Size;

}
public struct ImageAssetDescriptor
{
    public uint Width;
    public uint Height;
    public uint Format; //DXGI_FORMAT in current implementation
    public uint Stride;
}

public struct ModelAssetDescriptor
{
    public uint Vertices;
    public uint Indices;
    public byte IndexSize;
}

public struct ShaderAssetDescriptor
{

}
