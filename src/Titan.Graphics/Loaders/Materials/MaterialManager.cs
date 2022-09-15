using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Graphics.Loaders.Materials
{
    public unsafe class MaterialManager : IDisposable
    {
        private ResourcePool<Material> _resources;
        public MaterialManager(uint maxMaterials = 1000)
        {
            _resources.Init(maxMaterials);
        }

        public Handle<Material> Create(MaterialCreation args)
        {
            var handle = _resources.CreateResource();
            if(!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Material Handle");
            }

            var material = _resources.GetResourcePointer(handle);
            material->PixelShader = args.PixelShader;
            material->VertexShader = args.VertexShader;
            ref var properties = ref material->Properties;
            properties.AmbientColor = args.AmbientColor;
            properties.DiffuseColor = args.DiffuseColor;
            properties.SpecularColor = args.SpecularColor;
            properties.EmissiveColor = args.EmissiveColor;
            properties.DiffuseMap = args.DiffuseMap;
            properties.AmbientMap = args.AmbientMap;
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Material Access(in Handle<Material> handle) => ref _resources.GetResourceReference(handle);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release(in Handle<Material> handle)
        {
            Logger.Trace<MaterialManager>($"Releasing material with handle {handle}");
            _resources.ReleaseResource(handle);
        }

        public void Dispose()
        {
            Logger.Trace<MaterialManager>("Terminate resource pool");
            _resources.Terminate();
        }
    }
}
