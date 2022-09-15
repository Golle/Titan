using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.Assets
{
    public record AssetManagerConfiguration(string[] Manifests, int MaxConcurrentFileReads);

    public class AssetsManager
    {
        private readonly Dictionary<AssetTypes, IAssetLoader> _loaders = new();
        private Loader _loader;

        public AssetsManager Register(AssetTypes type, IAssetLoader loader)
        {
            Logger.Trace<AssetsManager>($"Added loader for asset type {type}");
            _loaders.Add(type, loader);
            return this;
        }

        public AssetsManager Init(AssetManagerConfiguration config)
        {
            var assets = config.Manifests
                .SelectMany(file =>
                {
                    var directory = Path.GetDirectoryName(file);
                    var manifest = AssetManifestDescriptor.CreateFromFile(file);
                    return manifest.Assets.Select(descriptor =>
                    {
                        if (!Enum.TryParse<AssetTypes>(descriptor.Type, true, out var type))
                        {
                            Logger.Error<AssetsManager>($"Failed to parse type {descriptor.Type} to the type {nameof(AssetTypes)}");
                        }

                        if (!_loaders.TryGetValue(type, out var loader))
                        {
                            Logger.Error<AssetsManager>($"No loader registered for the type {descriptor.Type} that was found in the manifest. Asset will be discarded.");
                            throw new InvalidOperationException("Missing loader");
                        }

                        var files = string.IsNullOrEmpty(directory) ? descriptor.Files : descriptor.Files?.Select(f => Path.Combine(directory, f)).ToArray();
                        return new Asset
                        {
                            Identifier = descriptor.Id,
                            Files = files,
                            Loader = loader,
                            Status = descriptor.Preload ? AssetStatus.LoadRequested : AssetStatus.Unloaded,
                            ReferenceCount = descriptor.Preload ? 1 : 0,
                            Static = descriptor.Static,
                            Type = type,
                            Dependencies = descriptor.Dependencies?.Select(d => new AssetDependency {Id = d.Id, Name = d.Name }).ToArray() ?? Array.Empty<AssetDependency>()
                        };
                    });

                    //// Find the index of the dependencies
                    //for (var i = 0; i < assets.Length; ++i)
                    //{
                    //    var descriptor = manifest.Assets[i];
                    //    ref var asset = ref assets[i];
                    //    asset.Dependencies = descriptor.Dependencies?.Select(d => new AssetDependency(IndexOf(d.Id), d.Name)).ToArray() ?? Array.Empty<AssetDependency>();
                    //}

                    //return assets;

                    
                }).ToArray();

            // Check for duplicate identifiers
            for (var i = 0; i < assets.Length; ++i)
            {
                for (var j = i + 1; j < assets.Length; ++j)
                {
                    if (assets[i].Identifier == assets[j].Identifier)
                    {
                        throw new InvalidOperationException($"Multiple assets with the same identifier. {assets[i].Identifier}");
                    }
                }
            }

            // Cache the index for the dependencies
            for (var i = 0; i < assets.Length; ++i)
            {
                ref var asset = ref assets[i];
                for (var j = 0; j < asset.Dependencies.Length; ++j)
                {
                    ref var dependency = ref asset.Dependencies[j];
                    dependency.Index = IndexOf(assets, dependency.Id);
                }
            }

            static int IndexOf(Asset[] assets, string id)
            {
                for (var i = 0; i < assets.Length; ++i)
                {
                    if (assets[i].Identifier == id)
                    {
                        return i;
                    }
                }
                throw new Exception($"Index for asset {id} could not be found.");
            }

            _loader = new Loader(assets, config.MaxConcurrentFileReads);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update() => _loader.Update();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Handle<Asset> Load(string identifer) => _loader.Load(identifer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unload(string identifier) => _loader.Unload(identifier);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Handle<T> GetAssetHandle<T>(in Handle<Asset> handle)
        {
#if DEBUG
            ref readonly var asset = ref _loader.GetAsset(handle);
            if (asset.Status != AssetStatus.Loaded)
            {
                Logger.Error<Loader>("Getting handle for Asset that is not loaded. Returning 0");
                return default;
            }
            return asset.AssetHandle;
#else
            return _loader.GetAsset(handle).AssetHandle;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLoaded(in Handle<Asset> handle) => _loader.GetAsset(handle).Status == AssetStatus.Loaded;
    }
}
