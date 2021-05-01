using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.Assets
{
    public record AssetManagerConfiguration(string Manifest, int MaxConcurrentFileReads);

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
            var manifest = AssetManifestDescriptor.CreateFromFile(config.Manifest);

            var assets = manifest.Assets.Select(descriptor =>
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

                return new Asset
                {
                    Identifier = descriptor.Name,
                    Files = descriptor.Files,
                    Loader = loader,
                    Status = descriptor.Preload ? AssetStatus.LoadRequested : AssetStatus.Unloaded,
                    ReferenceCount = descriptor.Preload ? 1 : 0,
                    Static = descriptor.Static,
                    Type =  type
                };
            }).ToArray();

            int IndexOf(string name)
            {
                for (var i = 0; i < assets.Length; ++i)
                {
                    if (assets[i].Identifier == name)
                    {
                        return i;
                    }
                }
                throw new Exception($"Index for asset {name} could not be found.");
            }

            // Get the index for the dependencies, for faster lookups
            for (var i = 0; i < assets.Length; ++i)
            {
                var descriptor = manifest.Assets[i];
                ref var asset = ref assets[i];
                asset.Dependencies = descriptor.Dependencies?.Select(IndexOf).ToArray() ?? Array.Empty<int>();
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
        public T GetAssetHandle<T>(in Handle<Asset> handle)
        {
#if DEBUG
            ref readonly var asset = ref _loader.GetAsset(handle);
            if (asset.Status != AssetStatus.Loaded)
            {
                Logger.Error<Loader>("Getting handle for Asset that is not loaded. Returning 0");
                return default;
            }
            return (T)asset.AssetReference;
#else
            return (T)_loader.GetAsset(handle).AssetReference;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLoaded(in Handle<Asset> handle) => _loader.GetAsset(handle).Status == AssetStatus.Loaded;
    }
}
