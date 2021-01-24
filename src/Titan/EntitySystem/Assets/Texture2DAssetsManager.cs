using System.Runtime.CompilerServices;
using Titan.Core.Threading;
using Titan.ECS.Assets;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.World;
using Titan.Graphics.Textures;

namespace Titan.EntitySystem.Assets
{
    public class Texture2DAssetsManager : AssetsManager<Texture>
    {
        private readonly ITextureLoader _loader;
        public Texture2DAssetsManager(ITextureLoader loader, WorkerPool workerPool, IWorld world, IEventManager eventManager) 
            : base(workerPool, world, eventManager)
        {
            _loader = loader;
        }
        protected override Texture Load(string identifier) => _loader.Load(identifier);
        protected override void Release(in Texture asset) => _loader.Release(asset);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnLoaded(in Texture asset, in Entity entity)
        {
            entity.AddComponent(asset);
        }
    }
}
