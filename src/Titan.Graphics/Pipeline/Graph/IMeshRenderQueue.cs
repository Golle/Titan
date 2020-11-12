using System;
using System.Numerics;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Texture = Titan.Graphics.Textures.Texture;

namespace Titan.Graphics.Pipeline.Graph
{
    public interface IMeshRenderQueue
    {
        void Submit(in Mesh mesh, in Matrix4x4 worldMatrix, Texture texture, MaterialHandle[] materials);
        ReadOnlySpan<Renderable> GetRenderables();
        void Reset();
    }
}
