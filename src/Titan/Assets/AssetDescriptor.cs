using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Assets;
public struct AssetDescriptor
{
    public uint Id;
    public uint ManifestId;
    public AssetDescriptorType Type;
    public AssetReference Reference;
    private AssetDescriptorUnion _union;

    [UnscopedRef] 
    public ref ImageAssetDescriptor Image => ref _union.ImageAssetDescriptor;
    [UnscopedRef] 
    public ref ModelAssetDescriptor Model => ref _union.ModelAssetDescriptor;
    [UnscopedRef]
    public ref ShaderAssetDescriptor Shader => ref _union.ShaderAssetDescriptor;
    [UnscopedRef]
    public ref AudioAssetDescriptor Audio => ref _union.AudioAssetDescriptor;

    [StructLayout(LayoutKind.Explicit)]
    private struct AssetDescriptorUnion
    {
        [FieldOffset(0)]
        public ImageAssetDescriptor ImageAssetDescriptor;
        [FieldOffset(0)]
        public ModelAssetDescriptor ModelAssetDescriptor;
        [FieldOffset(0)]
        public ShaderAssetDescriptor ShaderAssetDescriptor;
        [FieldOffset(0)]
        public AudioAssetDescriptor AudioAssetDescriptor;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ulong GetSize() => Reference.Size;
}
