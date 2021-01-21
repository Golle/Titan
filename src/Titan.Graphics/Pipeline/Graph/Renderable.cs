using System.Numerics;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Textures;

namespace Titan.Graphics.Pipeline.Graph
{
    public struct Renderable
    {
        public Texture Texture;
        public Mesh Mesh;
        public Matrix4x4 World;
        public MaterialHandle[] Materials;
    }
}
