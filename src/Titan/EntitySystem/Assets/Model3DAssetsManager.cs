using Titan.Core.Threading;
using Titan.ECS.Assets;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.World;
using Titan.EntitySystem.Components;
using Titan.Graphics.Meshes;

namespace Titan.EntitySystem.Assets
{
    internal class Model3DAssetsManager : AssetsManager<MeshComponent>
    {
        private readonly IMeshLoader _meshLoader;

        public Model3DAssetsManager(IMeshLoader meshLoader, WorkerPool workerPool, IWorld world, IEventManager eventManager) :
            base(workerPool, world, eventManager)
        {
            _meshLoader = meshLoader;
        }

        protected override MeshComponent Load(string identifier)
        {
            var mesh = _meshLoader.LoadMesh(identifier);
            //return new MeshComponent
            //{
            //    IndexBuffer = mesh
            //}
            return new MeshComponent();
            //throw new NotImplementedException();
        }

        protected override void Release(in MeshComponent asset)
        {
            //throw new NotImplementedException();
        }

        protected override void OnLoaded(in MeshComponent asset, in Entity entity)
        {
            
        }
    }
    
}
