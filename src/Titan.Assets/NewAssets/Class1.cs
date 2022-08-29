using System.Diagnostics;
using Titan.Core;
using Titan.ECS.App;

namespace Titan.Assets.NewAssets;

public struct AssetsModule : IModule
{
    public static void Build(AppBuilder builder)
    {

        var assetConfigs = builder.GetConfigurations<AssetsConfiguration>();

#if !SHIPPING 
        // never use paths for shipping config
        var devSettings = builder.GetConfiguration<AssetsDevConfiguration>();
        //
#endif
    }
}

/// <summary>
/// Configuration of paths during development, titanpak, tmanifest and assets. This is used in combination with AssetsConfiguration
/// In shipping builds the titanpak file is expected to be located in the same folder as the binary. (and no supprt for .tmanifest since the code for that will be stripped)
/// </summary>
/// <param name="AssetsFolder">The directory where the .tmanifest file is located</param>
public record AssetsDevConfiguration(string AssetsFolder, string TitanPakFolder) : IConfiguration;
public record AssetsConfiguration(string ManifestFile, string TitanPakFile) : IConfiguration
{
    /// <summary>
    /// Creates an asset configuration from a IManifestDescriptor and an optional path to the manifest directory.
    /// </summary>
    /// <typeparam name="TRegistry"></typeparam>

    /// <returns></returns>
    public static AssetsConfiguration CreateFromRegistry<TRegistry>() where TRegistry : IManifestDescriptor => new(TRegistry.ManifestFile, TRegistry.TitanPackageFile);
}


/*
 *Asset management system (V1, load .titanpak without support for hotreload)
 *
 * Register bundle (this will add the manifest (or titanpak) to the assets registry. (we could use reflection, but that might cause issues with nativeAOT)
 *
 *
 *
 */
