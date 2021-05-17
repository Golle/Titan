using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Assets;
using Titan.Assets.Materials;
using Titan.Assets.Models;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;
using Buffer = Titan.Graphics.D3D11.Buffers.Buffer;

namespace Titan.Rendering
{
    internal struct Renderable
    {
        public Matrix4x4 Transform;

        public Handle<Buffer> VertexBuffer;
        public Handle<Buffer> IndexBuffer;
        public Handle<Material> Material;
        public uint StartIndex;
        public uint Count;
    }
    
    internal class SimpleRenderQueue
    {
        public Renderable[] _renderables;
        public int _count;
        public SimpleRenderQueue(uint max)
        {
            _renderables = new Renderable[max];
        }

        public void Push(in Matrix4x4 transform, Model model)
        {
            for(var i = 0; i < model.Mesh.Submeshes.Length; ++i)
            {
                ref readonly var submesh = ref model.Mesh.Submeshes[i];
                _renderables[Interlocked.Increment(ref _count) - 1] = new Renderable
                {
                    Transform = transform,
                    Count = submesh.Count,
                    StartIndex = submesh.StartIndex,
                    Material = submesh.Material,
                    IndexBuffer = model.Mesh.IndexBuffer,
                    VertexBuffer = model.Mesh.VertexBuffer
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<Renderable> GetRenderables() => new(_renderables, 0, _count);

        public void Update()
        {
            _count = 0;
        }
    }


    internal class GeometryRenderer : IRenderer
    {
        private readonly SimpleRenderQueue _queue;
        private readonly Handle<Buffer> _transformBuffer;
        private readonly Handle<Buffer> _materialBuffer;

        public unsafe GeometryRenderer(SimpleRenderQueue queue)
        {
            _queue = queue;
            _transformBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = 1,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint) sizeof(Matrix4x4),
                Type = BufferTypes.ConstantBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });

            _materialBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = 1,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint) sizeof(MaterialBuffer),
                Type = BufferTypes.ConstantBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });
        }


        public void Render(Context context)
        {
            Unsafe.SkipInit(out MaterialBuffer materialBuffer);

            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexShaderConstantBuffer(_transformBuffer, 1);
            context.SetPixelShaderConstantBuffer(_materialBuffer, 1);
            foreach (ref readonly var renderable in _queue.GetRenderables())
            {
                context.Map(_transformBuffer, renderable.Transform);
                
                context.SetVertexBuffer(renderable.VertexBuffer);    
                context.SetIndexBuffer(renderable.IndexBuffer);

                ref readonly var material = ref Resources.Material.Access(renderable.Material);
                materialBuffer.DiffuseColor = material.Properties.DiffuseColor;
                context.Map(_materialBuffer, materialBuffer);

                context.DrawIndexed(renderable.Count, renderable.StartIndex);
            }
        }

        public void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_transformBuffer);
            GraphicsDevice.BufferManager.Release(_materialBuffer);
        }
    }

    internal struct MaterialBuffer
    {
        public Color DiffuseColor;
    }
}
