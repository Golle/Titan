namespace Titan.Assets;

internal interface IAssetFileReader
{
    ulong GetSizeFromDescriptor(in AssetDescriptor descriptor);
    int Read(Span<byte> buffer, in AssetDescriptor descriptor);
}
