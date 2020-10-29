using System;
using System.Numerics;
using Titan.Graphics.Meshes;

namespace Titan.Graphics.Pipeline.Graph
{
    public interface IMeshRenderQueue
    {

        void Submit(in Mesh mesh, in Matrix4x4 worldMatrix);
        ReadOnlySpan<Renderable> GetRenderables();
    }
    
    internal class NaiveMeshRenderQueue : IMeshRenderQueue
    {
        private readonly Renderable[] _renderables = new Renderable[10_000];
        private int _count;

        public void Submit(in Mesh mesh, in Matrix4x4 worldMatrix)
        {
            ref var renderable = ref _renderables[_count++];
            renderable.Mesh = mesh;
            renderable.World = worldMatrix;
        }
       
        public ReadOnlySpan<Renderable> GetRenderables() => new ReadOnlySpan<Renderable>(_renderables, 0, _count);


        public void Reset()
        {
            _count = 0;
        }
    }
}
