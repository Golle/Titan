using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
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
        public Model Model;
    }
    
    internal class SimpleRenderQueue
    {
        public Renderable[] _renderables;
        public int _count;
        public SimpleRenderQueue(uint max)
        {
            _renderables = new Renderable[max];
        }

        public void Push(in Matrix4x4 transform, Model model) => _renderables[Interlocked.Increment(ref _count) - 1] = new Renderable {Model = model, Transform = transform};

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
            Unsafe.SkipInit(out MaterialBuffer material);

            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexShaderConstantBuffer(_transformBuffer, 1);
            context.SetPixelShaderConstantBuffer(_materialBuffer, 1);
            foreach (ref readonly var renderable in _queue.GetRenderables())
            {
                ref readonly var mesh = ref renderable.Model.Mesh;

                context.Map(_transformBuffer, renderable.Transform);
                
                context.SetVertexBuffer(mesh.VertexBuffer);    
                context.SetIndexBuffer(mesh.IndexBuffer);

                for (var i = 0; i < mesh.Submeshes.Length; ++i)
                {
                    ref readonly var submesh = ref mesh.Submeshes[i];
                    material.DiffuseColor = submesh.Material.Properties.DiffuseColor;
                    context.Map(_materialBuffer, material);

                    context.DrawIndexed(submesh.Count, submesh.StartIndex);
                }
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
