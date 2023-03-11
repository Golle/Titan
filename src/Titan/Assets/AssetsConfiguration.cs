using Titan.Setup.Configs;

namespace Titan.Assets;

/// <summary>
/// Configuration of paths during development, titanpak, tmanifest and assets. This is used in combination with AssetsConfiguration
/// In shipping builds the titanpak file is expected to be located in the same folder as the binary. (and no supprt for .tmanifest since the code for that will be stripped)
/// </summary>
/// <param name="AssetsFolder">The directory where the .tmanifest file is located</param>
public record AssetsDevConfiguration(string AssetsFolder, string TitanPakFolder, string EnginePath, bool UseRawAssets) : IConfiguration
{
    public string EngineAssetsPath => Path.Combine(EnginePath, "assets");
    public string EngineBinPath => Path.Combine(EngineAssetsPath, "bin");
}
public record AssetsConfiguration(uint Id, string ManifestFile, string TitanPakFile, AssetDescriptor[] AssetDescriptors, object[] RawAssets, bool BuiltInAssets) : IConfiguration
{
    /// <summary>
    /// Creates an asset configuration from a IManifestDescriptor and an optional path to the manifest directory.
    /// </summary>
    /// <typeparam name="TRegistry"></typeparam>
    /// <returns>The configuration</returns>
    public static AssetsConfiguration CreateFromRegistry<TRegistry>(bool builtIn) where TRegistry : IManifestDescriptor
        => new(TRegistry.Id, TRegistry.ManifestFile, TRegistry.TitanPackageFile, TRegistry.AssetDescriptors, TRegistry.RawAssets, builtIn);

    internal AssetsConfiguration WithDevPaths(AssetsDevConfiguration config)
    {
        if (config == null)
        {
            // no changes if there's no dev config
            return this;
        }
        return this with
        {
            TitanPakFile = Path.Combine(BuiltInAssets ? config.EngineBinPath : config.TitanPakFolder, TitanPakFile),
            ManifestFile = Path.Combine(BuiltInAssets ? config.EngineAssetsPath : config.AssetsFolder, ManifestFile)
        };
    }
}
