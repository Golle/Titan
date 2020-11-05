using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal class DefaultLightsRenderer : IRenderer
    {
        private readonly ConstantBuffer<LightSource> _lightSource;
        private readonly IShaderManager _shaderManager;
        private readonly VertexBuffer<FullscreenVertex> _vertexBuffer;
        private readonly IndexBuffer<ushort> _indexBuffer;

        public DefaultLightsRenderer(IGraphicsDevice device, IShaderManager shaderManager)
        {
            _shaderManager = shaderManager;
            _vertexBuffer = new VertexBuffer<FullscreenVertex>(device, new[]
            {
                new FullscreenVertex {Position = new Vector2(-1, -1), UV = new Vector2(0, 1)},
                new FullscreenVertex {Position = new Vector2(-1, 1), UV = new Vector2(0, 0)},
                new FullscreenVertex {Position = new Vector2(1, 1), UV = new Vector2(1, 0)},
                new FullscreenVertex {Position = new Vector2(1, -1), UV = new Vector2(1, 1)},
            });
            _indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3 });

            _lightSource = new ConstantBuffer<LightSource>(device, new LightSource
            {
                Position = new Vector3(0, 0, -1)
            }, D3D11_USAGE.D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);
        }


        private Vector3 _lightPosition = new Vector3(0,0,-1);
        private float _lightVelocity = 0.05f;
        public void Render(IRenderContext context)
        {
            if (_lightPosition.X > 5.0f)
            {
                _lightPosition.X = 4.99f;
                _lightVelocity = -0.05f;
            }
            else if (_lightPosition.X < -5.0f)
            {
                _lightPosition.X = -4.99f;
                _lightVelocity = 0.05f;
            }

            _lightPosition.X += _lightVelocity;
            unsafe
            {
                context.MapResource(_lightSource.AsResourcePointer(), new LightSource {Position = _lightPosition});
            }

            context.SetPixelShaderConstantBuffer(_lightSource);

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBuffer);
            context.SetIndexBuffer(_indexBuffer);
            _shaderManager.Get(_shaderManager.GetHandle("DeferredShadingDefault")).Bind(context);

            context.DrawIndexed(6);

        }

        public void Dispose()
        {
            _lightSource.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }


        [StructLayout(LayoutKind.Sequential, Size = 48)]
        private struct LightSource
        {
            public Vector3 Position;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FullscreenVertex
        {
            public Vector2 Position;
            public Vector2 UV;
        }

    }
}
