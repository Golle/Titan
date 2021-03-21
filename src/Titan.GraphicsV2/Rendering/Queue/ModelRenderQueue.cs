using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.GraphicsV2.Resources;
using Titan.GraphicsV2.Resources.Bundles;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.GraphicsV2.Rendering.Queue
{
    internal struct Renderable
    {
        public Model3D Model;
        public Matrix4x4 World;
    }

    internal class ModelRenderQueue
    {
        private readonly Renderable[] _renderables = new Renderable[10000];
        private int _count;

        public void Enqueue(in Renderable renderable)
        {
            _renderables[_count++] = renderable;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<Renderable> GetRendereables() => new(_renderables, 0, _count);

        public void Reset() => _count = 0;
        
        public void SetActiveCamera(in Matrix4x4 view, in Matrix4x4 viewProjection)
        {
            View = view;
            ViewProjection = viewProjection;
        }

        public Matrix4x4 ViewProjection { get; private set; }
        public Matrix4x4 View { get; private set; }
    }
}
