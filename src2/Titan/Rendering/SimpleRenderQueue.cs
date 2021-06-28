using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Assets.Models;

namespace Titan.Rendering
{
    internal class SimpleRenderQueue
    {
        public Renderable[] _renderables;
        public int _count;
        public SimpleRenderQueue(uint max)
        {
            _renderables = new Renderable[max];
        }

        public void Push(in Matrix4x4 transform, Model model)
        {
            for(var i = 0; i < model.Mesh.Submeshes.Length; ++i)
            {
                ref readonly var submesh = ref model.Mesh.Submeshes[i];
                _renderables[Interlocked.Increment(ref _count) - 1] = new Renderable
                {
                    Transform = transform,
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
