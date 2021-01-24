using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.Core.Threading;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;
using Titan.ECS.Registry;
using Titan.ECS.World;

namespace Titan.ECS.Assets
{
    public abstract class AssetsManager<TComponent> : IAssetsManager, IDisposable where TComponent : unmanaged
    {
        private static readonly ComponentId AssetId = ComponentId<Asset<TComponent>>.Id;

        private readonly WorkerPool _workerPool;
        private readonly IEventManager _eventManager;
        private readonly IManagedComponentPool<Asset<TComponent>> _componentPool;
        private readonly Dictionary<string, TComponent> _loadedAssets = new();

        private readonly AssetProgress[] _activeLoaders = new AssetProgress[100];
        private int _activeLoadersCount;

        protected AssetsManager(WorkerPool workerPool, IWorld world, IEventManager eventManager)
        {
            _workerPool = workerPool;
            _componentPool = world.GetManagedComponentPool<Asset<TComponent>>();
            _eventManager = eventManager;
        }

        public void Update()
        {
            foreach (ref readonly var @event in _eventManager.GetEvents())
            {
                if (@event.Type == ComponentAddedEvent.Id)
                {
                    ref readonly var addedEvent = ref @event.As<ComponentAddedEvent>();
                    if (addedEvent.Component == AssetId)
                    {
                        ref var asset = ref _componentPool[addedEvent.Entity];
                        OnLoad(asset.Identifier, addedEvent.Entity);
                    }
                }
            }

            for (var i = 0; i < _activeLoadersCount; ++i)
            {
                ref var loader = ref _activeLoaders[i];
                if (loader.Handle.IsValid() && _workerPool.IsCompleted(loader.Handle))
                {
                    LOGGER.Trace("Asset loading finished: {0}", loader.Identifier);
                    _workerPool.Reset(ref loader.Handle);
                    _loadedAssets[loader.Identifier] = loader.Asset;
                    foreach (var entity in loader.WaitingEntities)
                    {
                        OnLoaded(loader.Asset, entity);
                    }

                    ref var lastLoader = ref _activeLoaders[--_activeLoadersCount];
                    _activeLoaders[i] = lastLoader;
                    lastLoader = default; // the GC will cleanup the List<T>
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnLoad(string identifier, in Entity entity)
        {
            LOGGER.Trace("Loading asset from : {0}", identifier);
            // Has the asset already been loaded?
            if (_loadedAssets.TryGetValue(identifier, out var asset))
            {
                OnLoaded(asset, entity);
                LOGGER.Trace("Asset already loaded, adding to entity", identifier);
                return;
            }

            // Do we have a running async operation for the asset already? add the entity to the waiting list.
            for (var i = 0; i < _activeLoadersCount; ++i)
            {
                ref readonly var loader = ref _activeLoaders[i];
                if (loader.Identifier == identifier)
                {
                    loader.WaitingEntities.Add(entity);
                    LOGGER.Trace("Asset already started loading, adding entity to waiting list", identifier);
                    return;
                }
            }

            // add it to the end of the array
            {
                var index = _activeLoadersCount++;
                ref var loader = ref _activeLoaders[index];
                loader.Identifier = identifier;
                loader.WaitingEntities = new List<Entity> { entity }; // will create gen2-3 garbage :<

                LOGGER.Trace("Asset has not been loaded, creating async job.", identifier);
                loader.Handle = _workerPool.Enqueue(new JobDescription(() => _activeLoaders[index].Asset = Load(identifier), autoReset: false)); // TODO: This will create a delegate, allocated on the heap. can it be prevented?;
            }
        }

        protected abstract TComponent Load(string identifier);
        protected abstract void Release(in TComponent asset);
        protected abstract void OnLoaded(in TComponent asset, in Entity entity);

        public void Dispose()
        {
            foreach (var asset in _loadedAssets.Values)
            {
                Release(asset);
            }
        }

        private struct AssetProgress
        {
            public string Identifier;
            public Handle<WorkerPool> Handle;
            public TComponent Asset;
            public List<Entity> WaitingEntities;
        }
    }
}
