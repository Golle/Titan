using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
            lock (_map) // TOOD: replace this with an action queue to avoid locks
            {
                var index = IndexOf(identifier);
                ref var asset = ref _assets[index];
                if (asset.Status == AssetStatus.Unloaded)
                {
                    asset.Status = AssetStatus.LoadRequested;
                }
                asset.ReferenceCount++;
                return index;
            }
        }

        public void Unload(string identifier)
        {
            lock (_map)
            {
                ref var asset = ref _assets[IndexOf(identifier)];
                if (asset.Static)
                {
                    Logger.Warning<Loader>("Trying to unload a static asset.");
                    return;
                }
                
                if (asset.Status is not AssetStatus.UnloadRequested or AssetStatus.Unloaded)
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
                    case AssetStatus.ReadingFile:
                    case AssetStatus.CreatingAsset:
                    case AssetStatus.Loaded:
                        break;
                    case AssetStatus.LoadRequested:
                        if (_fileReadCounts >= _maxConcurrentFileReads)
                        {
                            //Logger.Trace<Loader>($"Max file loads reached");
                            continue;
                        }
                        Interlocked.Increment(ref _fileReadCounts);
                        asset.Status = AssetStatus.ReadingFile;
                        LoadFile(i);
                        break;
                    
                    case AssetStatus.FileReadComplete:
                        Interlocked.Decrement(ref _fileReadCounts);
                        asset.Status = AssetStatus.CreatingAsset;
                        CreateAsset(i);
                        break;

                    case AssetStatus.RequestDependencies:
                        foreach (var dependency in asset.Dependencies)
                        {
                            Load(dependency);
                        }
                        asset.Status = AssetStatus.WaitingForDependencies;
                        break;

                    case AssetStatus.WaitingForDependencies:
                        UpdateDependency(ref asset);
                        break;

                    case AssetStatus.AssetCreated:
                        asset.FileBytes.Free();
                        asset.Status = asset.Dependencies.Length == 0 ? AssetStatus.Loaded : AssetStatus.RequestDependencies;
                        break;

                    case AssetStatus.UnloadRequested:
                        // TODO: should this be async?
                        asset.Loader.OnRelease(asset.AssetHandle);
                        foreach (var dependency in asset.Dependencies)
                        {
                            Unload(dependency);
                        }
                        asset.Status = AssetStatus.Unloaded;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateDependency(ref Asset asset)
        {
            foreach (var dependency in asset.Dependencies)
            {
                ref readonly var dependencyAsset = ref _assets[IndexOf(dependency)];
                if (dependencyAsset.Status != AssetStatus.Loaded)
                {
                    return;
                }
            }
            asset.Status = AssetStatus.Loaded;
        }

        private void CreateAsset(int index)
        {
            IOWorkerPool.QueueWorkerItem<(int, Asset[])>(static value =>
            {
                var (i, assets) = value;
                ref var asset = ref assets[i];
                Logger.Trace<Loader>($"Creating asset");
                asset.AssetHandle = asset.Loader.OnLoad(asset.FileBytes);
                Logger.Trace<Loader>($"Asset created");
                asset.Status = AssetStatus.AssetCreated;
            }, (index, _assets));
        }

        private void LoadFile(int index)
        {
            IOWorkerPool.QueueWorkerItem<(int, Asset[])>(static value =>
            {
                var (i, assets) = value;
                ref var asset = ref assets[i];
                Logger.Trace<Loader>($"File read starting {asset.Identifier}");
                using var file = FileSystem.OpenRead(asset.File);
                asset.FileBytes = MemoryUtils.AllocateBlock<byte>((uint) file.Length);
                file.Read(asset.FileBytes.AsSpan()); // TODO: do we need to handle smaller buffers?
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
