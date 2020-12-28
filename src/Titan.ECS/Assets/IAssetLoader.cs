using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;
using Titan.ECS.Registry;
using Titan.ECS.World;

namespace Titan.ECS.Assets
{
    public interface IAssetLoader<T> : IAssetLoader
    {
    }

    public interface IAssetLoader
    {
        void OnComponentAdded(in Entity entity);
        void OnComponentRemoved(in Entity entity);
        void OnEntityDestroyed(uint entityId);
    }

    public struct TextureComponent
    {
    }

    
    public class TextureAssetManager : AssetManager<TextureComponent>
    {
        public TextureAssetManager(IWorld world, IEventManager eventManager)
            : base(world, eventManager)
        {
        }


        protected override void OnLoaded(in Entity entity, in TextureComponent asset)
        {
            entity.AddComponent(asset);
        }

        protected override TextureComponent Load(string identifier)
        {
            return new TextureComponent();
        }
    }


    public abstract class AssetManager<T> where T : unmanaged
    {
        private readonly IEventManager _eventManager;
        private readonly IManagedComponentPool<Asset<T>> _assetPool;

        private readonly Dictionary<string, AssetTracker> _tracker = new ();
        
        protected AssetManager(IWorld world, IEventManager eventManager)
        {
            _eventManager = eventManager;
            _assetPool = world.GetManagedComponentPool<Asset<T>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void OnComponentAdded(in Entity entity)
        {
            ref readonly var component = ref _assetPool[entity];
            // TODO: replace this with a job system when we've got one of those implemented
            AssetTracker tracker;
            lock (_tracker)
            {
                if (_tracker.TryGetValue(component.Identifier, out tracker))
                {
                    tracker.References++;
                }
                else
                {
                    tracker = new AssetTracker(Load(component.Identifier));
                    _tracker[component.Identifier] = tracker;
                }
            }
            OnLoaded(entity, tracker.Asset);
        }
        
        protected abstract void OnLoaded(in Entity entity, in T asset);
        protected abstract T Load(string identifier);

        public void OnComponentRemoved(in Entity entity)
        {
            ref readonly var component = ref _assetPool[entity];
            // TODO: replace this with a job system when we've got one of those implemented
            AssetTracker tracker;
            lock (_tracker)
            {
                if (_tracker.TryGetValue(component.Identifier, out tracker))
                {
                    tracker.References++;
                }
                else
                {
                    tracker = new AssetTracker(Load(component.Identifier));
                    _tracker[component.Identifier] = tracker;
                }
            }
            OnLoaded(entity, tracker.Asset);
        }

        public void OnEntityBeingDestroyed(in Entity entity)
        {
            throw new System.NotImplementedException();
        }

        internal void Update()
        {
            foreach (ref readonly var @event in _eventManager.GetEvents())
            {
                if (@event.Type == ComponentAddedEvent.Id)
                {
                    ref readonly var componentAddedEvent = ref @event.As<ComponentAddedEvent>();
                    OnComponentAdded(componentAddedEvent.Entity);
                }
                else if (@event.Type == ComponentRemovedEvent.Id)
                {
                    ref readonly var componentRemovedEvent = ref @event.As<ComponentRemovedEvent>();
                    OnComponentRemoved(componentRemovedEvent.Entity);
                }
                else if (@event.Type == EntityBeingDestroyedEvent.Id)
                {
                    OnEntityBeingDestroyed(@event.As<EntityBeingDestroyedEvent>().Entity);
                }
            }
        }

        private class AssetTracker
        {
            internal readonly T Asset;
            internal int References;
            public AssetTracker(in T asset)
            {
                Asset = asset;
                References = 1;
            }
        }
    }
}
