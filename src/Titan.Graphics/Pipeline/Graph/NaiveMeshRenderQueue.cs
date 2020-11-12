using System;
using System.Drawing.Drawing2D;
using System.Numerics;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Texture = Titan.Graphics.Textures.Texture;

namespace Titan.Graphics.Pipeline.Graph
{


    internal class NaiveLightRenderQueue : ILigthRenderQueue
    {
        private readonly Light[] _lights = new Light[100];
        private int _numberOfLights = 0;
        public void Submit(in Vector3 worldPosition)
        {
            _lights[_numberOfLights++] = new Light {Position = worldPosition};
        }

        public ReadOnlySpan<Light> GetLights() => new ReadOnlySpan<Light>(_lights,0, _numberOfLights);

        public void Reset()
        {
            _numberOfLights = 0;
        }

        
    }
    public struct Light
    {
        public Vector3 Position;
    }

    internal interface ILigthRenderQueue
    {
        void Submit(in Vector3 worldPosition);
        ReadOnlySpan<Light> GetLights();
        public void Reset();
    }


    internal class NaiveMeshRenderQueue : IMeshRenderQueue
    {
        private readonly Renderable[] _renderables = new Renderable[10_000];
        private int _count;

        public void Submit(in Mesh mesh, in Matrix4x4 worldMatrix, Texture texture, MaterialHandle[] materials)
        {
            ref var renderable = ref _renderables[_count++];
            renderable.Mesh = mesh;
            renderable.World = worldMatrix;
            renderable.Texture = texture;
            renderable.Materials = materials;
        }
       
        public ReadOnlySpan<Renderable> GetRenderables() => new ReadOnlySpan<Renderable>(_renderables, 0, _count);


        public void Reset()
        {
            _count = 0;
        }
    }
}
