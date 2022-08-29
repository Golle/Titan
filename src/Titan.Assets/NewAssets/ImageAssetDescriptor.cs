using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Assets.NewAssets;


public enum AssetDescriptorType
{
    Texture,
    Model
}

public interface IManifestDescriptor
{
    static abstract uint Id { get; }
    static abstract string ManifestFile { get; }
    static abstract string TitanPackageFile { get; }
}
public unsafe struct AssetDescriptor
{
    public uint Id;
    public AssetDescriptorType Type;
    public AssetReference Reference;
    private AssetDescriptorUnion _union;
    public ref ImageAssetDescriptor Image => ref ((AssetDescriptorUnion*)Unsafe.AsPointer(ref _union))->ImageAssetDescriptor;
    public ref ModelAssetDescriptor Model => ref ((AssetDescriptorUnion*)Unsafe.AsPointer(ref _union))->ModelAssetDescriptor;

    [StructLayout(LayoutKind.Explicit)]
    private struct AssetDescriptorUnion
    {
        [FieldOffset(0)]
        public ImageAssetDescriptor ImageAssetDescriptor;
        [FieldOffset(0)]
        public ModelAssetDescriptor ModelAssetDescriptor;
    }

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
}
