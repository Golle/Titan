using System;
using Titan.Graphics.Meshes;

namespace Titan.Graphics.Pipeline.Graph
{
    public interface IMeshRenderQueue
    {
        ReadOnlySpan<Renderable> GetRenderables();
    }
    
    internal class NaiveMeshRenderQueue : IMeshRenderQueue
    {
        private readonly Renderable[] _renderables = new Renderable[10_000];
        private int _count;

        public void Submit(in Mesh mesh) => _renderables[_count++].Mesh = mesh; 
        public ReadOnlySpan<Renderable> GetRenderables() => new ReadOnlySpan<Renderable>(_renderables, 0, _count);


        public void Reset()
        {
            _count = 0;
        }
    }
}
