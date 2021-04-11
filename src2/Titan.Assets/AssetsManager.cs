using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.Assets
{
    public record AssetManagerConfiguration(string Manifest, int MaxConcurrentFileReads);

    public class AssetsManager
    {
        private readonly Dictionary<string, IAssetLoader> _loaders = new();
        private Loader _loader;

        public AssetsManager Register(IAssetLoader loader)
        {
            Logger.Trace<AssetsManager>($"Added loader for asset type {loader.Type}");
            _loaders.Add(loader.Type, loader);
            return this;
        }

        public AssetsManager Init(AssetManagerConfiguration config)
        {
            var manifest = AssetManifest.CreateFromFile(config.Manifest);

            var assets = new Asset[manifest.Assets.Length];

            var i = 0;
            foreach (var descriptor in manifest.Assets)
            {
                if (!_loaders.TryGetValue(descriptor.Type, out var loader))
                {
                    Logger.Warning<AssetsManager>($"No loader registered for the type {descriptor.Type} that was found in the manifest. Asset will be discarded.");
                }
                else
                {
                    assets[i++] = new Asset
                    {
                        Identifier = descriptor.Name,
                        File = Path.Combine(descriptor.File),
                        Loader = loader,
                        Status = descriptor.Preload ? AssetStatus.LoadRequested : AssetStatus.Unloaded,
                        AssetHandle = Handle<Asset>.Null,
                        Static = descriptor.Static,
                        Dependencies = descriptor.Dependencies ?? Array.Empty<string>()
                    };
                }
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
                return 0;
            }
            return asset.AssetHandle;
#else
            return _assets[handle.Value].AssetHandle;
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLoaded(in Handle<Asset> handle) => _loader.GetAsset(handle).Status == AssetStatus.Loaded;
    }
}
