using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Meshes
{
    public record Mesh(VertexBufferHandle VertexBufferHandle, IIndexBuffer IndexBuffer, SubMesh[] SubSets)
    {
        public uint NumberOfIndices
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IndexBuffer.NumberOfIndices;
        }

        //public void Dispose()
        //{
        //    VertexBuffer.Dispose();
        //    IndexBuffer.Dispose();
        //}
    }
}
