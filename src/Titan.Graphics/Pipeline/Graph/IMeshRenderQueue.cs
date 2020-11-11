using System;
using System.Numerics;
using Titan.Graphics.Meshes;
using Titan.Graphics.Textures;

namespace Titan.Graphics.Pipeline.Graph
{
    public interface IMeshRenderQueue
    {
        void Submit(in Mesh mesh, in Matrix4x4 worldMatrix, Texture texture);
        ReadOnlySpan<Renderable> GetRenderables();
        void Reset();
    }
}
