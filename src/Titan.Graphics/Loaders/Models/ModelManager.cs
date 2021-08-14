using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Graphics.Loaders.Models
{
    public ref struct ModelCreation
    {
        public Handle<ResourceBuffer> VertexBuffer;
        public Handle<ResourceBuffer> IndexBuffer;
        public ReadOnlySpan<Submesh> Submeshes;
    }

    public unsafe class ModelManager : IDisposable
    {
        private ResourcePool<Model> _resources;
        public ModelManager(uint maxModels = 1000)
        {
            _resources.Init(maxModels);
        }

        public Handle<Model> Create(in ModelCreation args)
        {
            var handle = _resources.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Model Handle");
            }

            var model = _resources.GetResourcePointer(handle);
            model->Mesh.Submeshes = MemoryUtils.AllocateBlock<Submesh>((uint)args.Submeshes.Length);
            var size = model->Mesh.Submeshes.Size;
            fixed (Submesh* pSubmeshes = args.Submeshes)
            {
                System.Buffer.MemoryCopy(pSubmeshes, model->Mesh.Submeshes.AsPointer(), size, size);
            }
            model->Mesh.IndexBuffer = args.IndexBuffer;
            model->Mesh.VertexBuffer = args.VertexBuffer;
            
            return handle;
        }

        public void Release(in Handle<Model> handle)
        {
            var model = _resources.GetResourcePointer(handle);
            model->Mesh.Submeshes.Free();
            _resources.ReleaseResource(handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Model Access(in Handle<Model> handle) => ref _resources.GetResourceReference(handle);

        public void Dispose()
        {
            foreach (var resource in _resources.EnumerateUsedResources())
            {
                _resources.GetResourcePointer(resource.Value)->Mesh.Submeshes.Free();
            }
            _resources.Terminate();
        }
    }
}
