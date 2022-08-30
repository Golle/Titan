using System;
using Titan.Core;

namespace Titan.Assets.NewAssets;

/// <summary>
/// Configuration of paths during development, titanpak, tmanifest and assets. This is used in combination with AssetsConfiguration
/// In shipping builds the titanpak file is expected to be located in the same folder as the binary. (and no supprt for .tmanifest since the code for that will be stripped)
/// </summary>
/// <param name="AssetsFolder">The directory where the .tmanifest file is located</param>
public record AssetsDevConfiguration(string AssetsFolder, string TitanPakFolder) : IConfiguration;

public record AssetsConfiguration(uint Id, string ManifestFile, string TitanPakFile, AssetDescriptor[] AssetDescriptors) : IConfiguration
{
    /// <summary>
    /// Creates an asset configuration from a IManifestDescriptor and an optional path to the manifest directory.
    /// </summary>
    /// <typeparam name="TRegistry"></typeparam>
    /// <returns></returns>
    public static AssetsConfiguration CreateFromRegistry<TRegistry>() where TRegistry : IManifestDescriptor
        => new(TRegistry.Id, TRegistry.ManifestFile, TRegistry.TitanPackageFile, TRegistry.AssetDescriptors);
}
