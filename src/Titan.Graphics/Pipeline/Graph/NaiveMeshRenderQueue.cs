using System;
using System.Numerics;
using Titan.Graphics.Meshes;
using Titan.Graphics.Textures;

namespace Titan.Graphics.Pipeline.Graph
{
    internal class NaiveMeshRenderQueue : IMeshRenderQueue
    {
        private readonly Renderable[] _renderables = new Renderable[10_000];
        private int _count;

        public void Submit(in Mesh mesh, in Matrix4x4 worldMatrix, Texture texture)
        {
            ref var renderable = ref _renderables[_count++];
            renderable.Mesh = mesh;
            renderable.World = worldMatrix;
            renderable.Texture = texture;
        }
       
        public ReadOnlySpan<Renderable> GetRenderables() => new ReadOnlySpan<Renderable>(_renderables, 0, _count);


        public void Reset()
        {
            _count = 0;
        }
    }
}
