using System.Runtime.InteropServices;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;
using Titan.GraphicsV2.Rendering.Queue;
using Titan.GraphicsV2.Resources;
using Titan.GraphicsV2.Resources.Bundles;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.EntitySystem
{

    [StructLayout(LayoutKind.Sequential, Size = 10)]
    public struct TEMPModel3D { }
    internal class Render3DSystem : SystemBase
    {
        private readonly ModelLoader _modelLoader;
        private readonly BundleLoader _bundleLoader;
        private readonly ModelRenderQueue _queue;
        private readonly ReadOnlyStorage<Transform3D> _transform;
        private bool _loaded;
        //private Model3D _model;
        private IEntityFilter _filter;
        private Bundle _bundle;

        public Render3DSystem(IWorld world, IEntityFilterManager entityFilterManager, ModelLoader modelLoader, BundleLoader bundleLoader, ModelRenderQueue queue) : base(world)
        {
            _modelLoader = modelLoader;
            _bundleLoader = bundleLoader;
            _queue = queue;
            _transform = GetRead<Transform3D>();
            _filter = entityFilterManager.Create(new EntityFilterConfiguration().With<Transform3D>().With<TEMPModel3D>());
        }

        public override void OnUpdate()
        {
            if (_loaded == false)
            {
                //_model = _modelLoader.Load("models1/clock_obj");
                _loaded = true;

                _bundle = _bundleLoader.Load("bundles/bundle01");
            }

            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);

                foreach (var bundleModel in _bundle.Models)
                {
                    _queue.Enqueue(new Renderable
                    {
                        Model = bundleModel,
                        World = transform.WorldMatrix
                    });
                }
                
            }
            
        }
    }
}
