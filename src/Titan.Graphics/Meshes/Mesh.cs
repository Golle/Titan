using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Graphics.Meshes
{
    public record Mesh(IVertexBuffer VertexBuffer, IIndexBuffer IndexBuffer, SubMesh[] SubSets) : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind(IRenderContext context)
        {
            context.SetVertexBuffer(VertexBuffer);
            context.SetIndexBuffer(IndexBuffer);
        }

        public uint NumberOfIndices
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IndexBuffer.NumberOfIndices;
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
