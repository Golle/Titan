using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading;

namespace Titan.Assets
{


    // Request load
    // 1. Check cache (has it already been loaded) -> increase ref count
    // 2. Check current loaders (is it being loaded right now) -> increase ref count
    // 3. Create new handle 
    // 4. Start load

    // IsLoaded(handle)
    // 


    public class Loader2
    {
        private Dictionary<string, IAsset> _loaders = new();
        public void RegisterLoader<T>() where T : unmanaged, IAsset
        {
            Logger.Trace<Loader2>($"Registered {typeof(T).Name}");
            _loaders.Add(typeof(T).Name, default(T));
        }

        public Handle<T> RequestLoad<T>(string identifier) where T : unmanaged, IAsset
        {
            return 0;
        }
    }


    public class Loader
    {
        private readonly int _maxConcurrentFileReads;
        private int _fileReadCounts;
        private readonly AssetState[] _assetStates = new AssetState[1000];
        private int _assetStateCount;
        
        public Loader(int maxConcurrentFileReads)
        {
            _maxConcurrentFileReads = maxConcurrentFileReads;
        }

        public int RequestLoad<T>(string identifier) where T : unmanaged, IAsset
        {
            var index = Interlocked.Increment(ref _assetStateCount) - 1;
            
            _assetStates[index] = new AssetState
            {
                State = LoadState.LoadRequested,
                Identifier = identifier,
                Asset = default(T)
            };

            return 0;
        }


        public void ProcessState()
        {
            for (var i = 0; i < _assetStateCount; ++i)
            {
                ref var assetState = ref _assetStates[i];
                switch (assetState.State)
                {
                    case LoadState.LoadRequested:
                        if (_fileReadCounts >= _maxConcurrentFileReads)
                        {
                            Logger.Trace<Loader>($"Max file loads reached");
                            continue;
                        }

                        Interlocked.Increment(ref _fileReadCounts);
                        LoadFile(i, assetState.Identifier);
                        assetState.State = LoadState.ReadingFile;
                        break;
                    case LoadState.ReadingFile:
                        // Ignored (Handled in the background)
                        break;
                    case LoadState.FileReadCompleted:
                        Interlocked.Decrement(ref _fileReadCounts);
                        var bytes = assetState.FileBytes.AsSpan();
                        Logger.Trace<Loader>($"Finished loading file {assetState.Identifier} with {bytes.Length}");
                        assetState.State = LoadState.RequestDependencies;
                        break;
                    case LoadState.RequestDependencies:
                        if (FileSystem.Exist($"{assetState.Identifier}.meta"))
                        {
                            // TODO: add dependencies implementation
                            assetState.State = LoadState.WaitingForDependencies;
                        }
                        else
                        {
                            assetState.State = LoadState.CreatingAsset;
                        }
                        break;
                    
                    case LoadState.WaitingForDependencies:
                        assetState.State = LoadState.CreatingAsset;
                        break;
                    case LoadState.CreatingAsset:
                        CreateAsset(i);
                        assetState.State = LoadState.WaitingForAsset;
                        break;
                    case LoadState.WaitingForAsset:
                        break;
                    case LoadState.CachingAsset:

                        assetState.State = LoadState.Loaded;
                        break;
                    case LoadState.Loaded:
                        Logger.Trace<Loader>("Asset loaded! unloading..");
                        assetState.State = LoadState.UnloadRequested;
                        break;
                    case LoadState.UnloadRequested:
                        Logger.Trace<Loader>("Unload requested");
                        assetState.Asset.OnRelease();
                        assetState.State = LoadState.Unloaded;
                        break;
                    case LoadState.Unloaded:
                        //Logger.Trace<Loader>("Asset unloaded");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


            }
            

        }

        private void CreateAsset(int index)
        {
            IOWorkerPool.QueueWorkerItem<(int, AssetState[])>(static value =>
            {
                var (i, assetStates) = value;
                ref var state = ref assetStates[i];
                Logger.Trace<Loader>($"Creating asset");
                state.Asset.OnLoad(state.FileBytes);
                Logger.Trace<Loader>($"Asset created");
                state.State = LoadState.CachingAsset;
                Logger.Trace<Loader>($"Free file bytes");
                state.FileBytes.Free();
            }, (index, _assetStates));
        }

        private void LoadFile(int index, string identifier)
        {
            IOWorkerPool.QueueWorkerItem<(int, AssetState[], string)>(static value =>
            {
                var (i, assetStates, id) = value;
                ref var state = ref assetStates[i];
                Logger.Trace<Loader>($"File read starting {state.Identifier}");
                using var file = FileSystem.OpenRead(id);
                state.FileBytes = MemoryUtils.AllocateBlock<byte>((uint) file.Length);
                file.Read(state.FileBytes.AsSpan()); // TODO: do we need to handle smaller buffers?
                Logger.Trace<Loader>($"File read finished {state.Identifier}");
                state.State = LoadState.FileReadCompleted;
            }, (index,  _assetStates, identifier));
        }
    }
}
