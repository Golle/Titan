using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.Core.Threading;
using Titan.ECS.Assets;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;
using Titan.ECS.Registry;
using Titan.ECS.World;
using Titan.Graphics.Textures;

namespace Titan.EntitySystem.Assets
{
    public class Texture2DAssetsManager : IAssetTestInterface, IDisposable
    {
        private struct AssetProgress
        {
            public string Identifier;
            public Handle<WorkerPool> Handle;
            public Texture Texture;
            public List<Entity> WaitingEntities;
        }

        private readonly WorkerPool _workerPool;
        private readonly ITextureLoader _textureLoader;
        private readonly IEventManager _eventManager;
        private readonly Dictionary<string, Texture> _loadedAssets = new();

        private readonly AssetProgress[] _activeLoaders = new AssetProgress[100];
        private int _activeLoadersCount;

        private readonly ComponentId _componentId = ComponentId<Asset<Texture>>.Id;
        private readonly IManagedComponentPool<Asset<Texture>> _pool;

        public Texture2DAssetsManager(WorkerPool workerPool, ITextureLoader textureLoader, IWorld world, IEventManager eventManager)
        {
            _workerPool = workerPool;
            _textureLoader = textureLoader;
            _pool = world.GetManagedComponentPool<Asset<Texture>>();
            _eventManager = eventManager;
            for (var i = 0; i < _activeLoaders.Length; ++i)
            {
                _activeLoaders[i].Handle = -1;
            }
        }

        public void Update()
        {
            foreach (ref readonly var @event in _eventManager.GetEvents())
            {
                if (@event.Type == ComponentAddedEvent.Id)
                {
                    ref readonly var addedEvent = ref @event.As<ComponentAddedEvent>();
                    if (addedEvent.Component == _componentId)
                    {
                        ref var asset = ref _pool[addedEvent.Entity];
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
                    _loadedAssets[loader.Identifier] = loader.Texture;
                    foreach (var entity in loader.WaitingEntities)
                    {
                        entity.AddComponent(loader.Texture);
                    }
                    _activeLoaders[i] = _activeLoaders[--_activeLoadersCount];
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
                entity.AddComponent(asset);
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
                if (loader.WaitingEntities != null)
                {
                    loader.WaitingEntities.Clear();
                    loader.WaitingEntities.Add(entity);
                }
                else
                {
                    loader.WaitingEntities = new List<Entity> { entity };
                }

                LOGGER.Trace("Asset has not been loaded, creating async job.", identifier);
                loader.Handle = _workerPool.Enqueue(new JobDescription(() => _activeLoaders[index].Texture = _textureLoader.Load(identifier), autoReset: false)); // TODO: This will create a delegate, allocated on the heap. can it be prevented?;
            }
        }

        public void Dispose()
        {
            foreach (var asset in _loadedAssets.Values)
            {
                _textureLoader.Release(asset);
            }
        }
    }
}
