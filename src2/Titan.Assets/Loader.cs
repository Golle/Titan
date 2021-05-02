using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading;

namespace Titan.Assets
{

    public class Loader
    {
        private readonly int _maxConcurrentFileReads;
        private volatile int _fileReadCounts;
        private readonly Asset[] _assets;
        private readonly Dictionary<string, int> _map;

        public Loader(Asset[] assets, int maxConcurrentFileReads)
        {
            _assets = assets;
            _map = _assets
                .Select((asset, index) => (asset, index))
                .ToDictionary(tuple => tuple.asset.Identifier, tuple => tuple.index);

            _maxConcurrentFileReads = maxConcurrentFileReads;
        }

        // TODO: handle invalid handles (_assets is 0 indexed right now)
        public Handle<Asset> Load(string identifier)
        {
            var index = IndexOf(identifier);
            Load(index);
            return index;
        }

        private void Load(int index)
        {
            lock (_map) // TOOD: replace this with an action queue to avoid locks
            { 
                ref var asset = ref _assets[index];
                if (asset.Status == AssetStatus.Unloaded)
                {
                    asset.Status = AssetStatus.LoadRequested;
                }
                asset.ReferenceCount++;
            }
        }

        public void Unload(string identifier) => Unload(IndexOf(identifier));
        private void Unload(int index)
        {
            lock (_map)
            {
                ref var asset = ref _assets[index];
                if (asset.Static)
                {
                    Logger.Warning<Loader>("Trying to unload a static asset.");
                    return;
                }

                if (asset.Status != AssetStatus.UnloadRequested && asset.Status != AssetStatus.Unloaded)
                {
                    asset.ReferenceCount--;
                    if (asset.ReferenceCount <= 0)
                    {
                        asset.Status = AssetStatus.UnloadRequested;
                    }
                }
                else
                {
                    Logger.Warning<Loader>("Tried to unload asset that has already been unloaded or flagged for being unloaded.");
                }
            }
        }


        public void Update()
        {
            ProcessState();
        }

        private void ProcessState()
        {
            for(var i = 0; i < _assets.Length; ++i)
            {
                ref var asset = ref _assets[i];

                switch (asset.Status)
                {
                    // Ignored by ProcessState
                    case AssetStatus.Unloaded:
                    case AssetStatus.ReadingFiles:
                    case AssetStatus.CreatingAsset:
                    case AssetStatus.Loaded:
                        break;
                    case AssetStatus.LoadRequested:
                        // Load dependencies first
                        if (asset.Dependencies.Length > 0)
                        {
                            foreach (var dependency in asset.Dependencies)
                            {
                                Load(dependency.Index);
                            }
                            asset.Status = AssetStatus.WaitingForDependencies;
                        }
                        else
                        {
                            asset.Status = AssetStatus.DependenciesLoaded;
                        }
                        break;

                    case AssetStatus.WaitingForDependencies:
                        if (DependenciesLoaded(asset.Dependencies))
                        {
                            asset.Status = AssetStatus.DependenciesLoaded;
                        }
                        break;

                    case AssetStatus.DependenciesLoaded:
                        // An asset could just be a set of dependencies, like a shader for example which consists of VertexShader, PixelShader and an InputLayout. 
                        if (asset.Files?.Length > 0)
                        {
                            asset.Status = AssetStatus.ReadingFiles;
                            LoadFiles(i);
                        }
                        else
                        {
                            asset.Status = AssetStatus.FileReadComplete;
                        }
                        break;

                    case AssetStatus.FileReadComplete:
                        asset.Status = AssetStatus.CreatingAsset;
                        CreateAsset(i);
                        break;

                    case AssetStatus.AssetCreated:
                        for (var fileIndex = 0; fileIndex < asset.FileBytes?.Length; ++fileIndex)
                        {
                            asset.FileBytes[fileIndex].Free();
                        }
                        asset.Status = asset.Status = AssetStatus.Loaded;
                        break;

                    case AssetStatus.UnloadRequested:
                        // TODO: should this be async?
                        asset.Loader.OnRelease(asset.AssetReference);
                        foreach (var dependency in asset.Dependencies)
                        {
                            Unload(dependency.Index);
                        }
                        asset.Status = AssetStatus.Unloaded;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool DependenciesLoaded(ReadOnlySpan<AssetDependency> dependencies)
        {
            foreach (ref readonly var dependency in dependencies)
            {
                ref readonly var dependencyAsset = ref _assets[dependency.Index];
                if (dependencyAsset.Status != AssetStatus.Loaded)
                {
                    return false;
                }
            }
            return true;
        }

        private void CreateAsset(int index)
        {
            IOWorkerPool.QueueWorkerItem<(int, Asset[])>(static value =>
            {
                var (assetIndex, assets) = value;
                ref var asset = ref assets[assetIndex];
                Logger.Trace<Loader>($"Creating asset");

                var dependencies = new Dependency[asset.Dependencies.Length]; // TODO: heap allocation, maybe its fine. Short lived.
                for (var i = 0; i < dependencies.Length; ++i)
                {
                    ref readonly var dependency = ref asset.Dependencies[i];
                    ref readonly var dep = ref assets[dependency.Index];
                    dependencies[i] = new Dependency(dep.Type, dep.Identifier, dependency.Name, dep.AssetReference);
                }
                asset.AssetReference = asset.Loader.OnLoad(asset.FileBytes, dependencies);
                Logger.Trace<Loader>($"Asset created");
                asset.Status = AssetStatus.AssetCreated;
            }, (index, _assets));
        }

        private void LoadFiles(int index)
        {
            IOWorkerPool.QueueWorkerItem<(int, Asset[])>(static value =>
            {
                var (assetIndex, assets) = value;
                ref var asset = ref assets[assetIndex];
                Logger.Trace<Loader>($"File read starting {asset.Identifier}");
                asset.FileBytes = new MemoryChunk<byte>[asset.Files.Length];
                for(var i = 0; i < asset.Files.Length; ++i)
                {
                    var path = asset.Files[i];
                    using var file = FileSystem.OpenRead(path);
                    var block = MemoryUtils.AllocateBlock<byte>((uint)file.Length);
                    file.Read(block.AsSpan());
                    asset.FileBytes[i] = block;
                }
                
                Logger.Trace<Loader>($"File read finished {asset.Identifier}");
                asset.Status= AssetStatus.FileReadComplete;
            }, (index, _assets));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IndexOf(string identifier)
        {
#if DEBUG
            if (!_map.TryGetValue(identifier, out var index))
            {
                Logger.Error<Loader>($"Tried to get index of asset {identifier} that didn't exist.");
                throw new InvalidOperationException($"Tried to get index of asset {identifier} that didn't exist.");
            }
            return index;
#else
            return  _map[identifier];
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly Asset GetAsset(in Handle<Asset> handle) => ref _assets[handle.Value];
    }
}
