using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Loaders.Materials;

namespace Titan.Graphics.Rendering.Geometry
{
    [SkipLocalsInit]
    public struct Renderable
    {
        public Matrix4x4 Transform;

        public Handle<ResourceBuffer> VertexBuffer;
        public Handle<ResourceBuffer> IndexBuffer;
        public Handle<Material> Material;
        public uint StartIndex;
        public uint Count;
    }
}
