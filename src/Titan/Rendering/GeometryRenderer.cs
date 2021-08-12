using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Assets;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Loaders;
using Titan.Windows.D3D11;

namespace Titan.Rendering
{
    internal sealed class GeometryRenderer : Renderer
    {
        private readonly SimpleRenderQueue _queue;
        private readonly Handle<ResourceBuffer> _transformBuffer;
        private readonly Handle<ResourceBuffer> _materialBuffer;

        private const uint TransformSlot = 5u;

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


        public override void Render(Context context)
        {
            Unsafe.SkipInit(out MaterialBuffer materialBuffer);

            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexShaderConstantBuffer(_transformBuffer, TransformSlot);
            context.SetPixelShaderConstantBuffer(_materialBuffer, 1);
            foreach (ref readonly var renderable in _queue.GetRenderables())
            {
                context.Map(_transformBuffer, renderable.Transform);
                
                context.SetVertexBuffer(renderable.VertexBuffer);    
                context.SetIndexBuffer(renderable.IndexBuffer);

                ref readonly var material = ref Resources.Material.Access(renderable.Material);
                context.SetPixelShader(material.PixelShader);
                context.SetVertexShader(material.VertexShader);

                ref readonly var properties = ref material.Properties;
                materialBuffer.DiffuseColor = properties.DiffuseColor;
                materialBuffer.Shininess = properties.Shininess;
                context.Map(_materialBuffer, materialBuffer);

                context.DrawIndexed(renderable.Count, renderable.StartIndex);
            }
        }

        public override void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_transformBuffer);
            GraphicsDevice.BufferManager.Release(_materialBuffer);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 256)]
    internal struct MaterialBuffer
    {
        public Color DiffuseColor;
        public float Shininess;
    }
}
