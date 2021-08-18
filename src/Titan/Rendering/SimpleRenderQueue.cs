using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Graphics.Loaders;
using Titan.Graphics.Loaders.Models;

namespace Titan.Rendering
{
    internal class SimpleRenderQueue
    {
        private readonly Renderable[] _renderables;
        private int _count;
        public SimpleRenderQueue(uint max)
        {
            _renderables = new Renderable[max];
        }

        public void Push(in Matrix4x4 transform, Handle<Model> handle)
        {
            ref readonly var model = ref Resources.Models.Access(handle);

            for(var i = 0; i < model.Mesh.Submeshes.Count; ++i)
            {
                ref readonly var submesh = ref model.Mesh.Submeshes[i];
                _renderables[Interlocked.Increment(ref _count) - 1] = new Renderable
                {
                    Transform = Matrix4x4.Transpose(transform),
                    Count = submesh.Count,
                    StartIndex = submesh.StartIndex,
                    Material = submesh.Material,
                    IndexBuffer = model.Mesh.IndexBuffer,
                    VertexBuffer = model.Mesh.VertexBuffer
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<Renderable> GetRenderables() => new(_renderables, 0, _count);

        public void Update()
        {
            _count = 0;
        }
    }
}
