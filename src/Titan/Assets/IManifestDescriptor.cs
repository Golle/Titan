namespace Titan.Assets;

public interface IManifestDescriptor
{
    static abstract uint Id { get; }
    static abstract string ManifestFile { get; }
    static abstract string TitanPackageFile { get; }
    static abstract uint AssetCount { get; }
    static abstract AssetDescriptor[] AssetDescriptors { get; }
    static abstract object[] RawAssets { get; }
}
