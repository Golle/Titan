using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal class DefaultLightsRenderer : IRenderer
    {
        private readonly ILigthRenderQueue _ligthRenderQueue;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;
        private readonly IConstantBufferManager _constantBufferManager;
        private readonly IShaderManager _shaderManager;

        private readonly Handle<VertexBuffer> _vertexBuffer;
        private readonly Handle<IndexBuffer> _indexBuffer;
        private readonly Handle<ConstantBuffer> _lightSourceHandle;
        private readonly ShaderProgram _shader;
        
        public unsafe DefaultLightsRenderer(ILigthRenderQueue ligthRenderQueue, IVertexBufferManager vertexBufferManager, IIndexBufferManager indexBufferManager, IConstantBufferManager constantBufferManager, IShaderManager shaderManager)
        {
            _ligthRenderQueue = ligthRenderQueue;
            _vertexBufferManager = vertexBufferManager;
            _indexBufferManager = indexBufferManager;
            _constantBufferManager = constantBufferManager;
            _shaderManager = shaderManager;

            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), UV = new Vector2(0, 1) };
            vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), UV = new Vector2(0, 0) };
            vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), UV = new Vector2(1, 0) };
            vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), UV = new Vector2(1, 1) };
            _vertexBuffer = _vertexBufferManager.CreateVertexBuffer(4u, (uint)sizeof(FullscreenVertex), vertices);
            
            var indices = stackalloc ushort[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            _indexBuffer = _indexBufferManager.CreateIndexBuffer<ushort>(6, indices);

            _lightSourceHandle = _constantBufferManager.CreateConstantBuffer(new LightSource(), D3D11_USAGE.D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);

            _shader = shaderManager.GetByName("DeferredShadingDefault");
        }

        public void Render(IRenderContext context)
        {
            ref readonly var lightSource = ref _constantBufferManager[_lightSourceHandle];
            unsafe
            {
                var lights = _ligthRenderQueue.GetLights();
                var light = new LightSource {NumberOfLights = lights.Length};
                for (var i = 0; i < light.NumberOfLights; ++i)
                {
                    light.Set(i, lights[i].Position);
                }
                context.MapResource(lightSource.Resource, light);
            }
            
            context.SetPixelShaderConstantBuffer(lightSource);
            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBufferManager[_vertexBuffer]);
            context.SetIndexBuffer(_indexBufferManager[_indexBuffer]);

            context.SetInputLayout(_shaderManager[_shader.InputLayout]);
            context.SetVertexShader(_shaderManager[_shader.VertexShader]);
            context.SetPixelShader(_shaderManager[_shader.PixelShader]);

            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            _constantBufferManager.DestroyBuffer(_lightSourceHandle);
            _vertexBufferManager.DestroyBuffer(_vertexBuffer);
            _indexBufferManager.DestroyBuffer(_indexBuffer);
        }

       
        [StructLayout(LayoutKind.Explicit)]
        private struct LightSource
        {
            private const int MaxNumberOfLights = 32;
            [FieldOffset(0)]
            public int NumberOfLights;
            [FieldOffset(16)] 
            private unsafe fixed float Position[4*MaxNumberOfLights];
            public unsafe void Set(int index, in Vector3 position)
            {
                fixed (Vector3* pPosition = &position)
                fixed (float* pLights = Position)
                {
                    Unsafe.CopyBlock(pLights + (index * 4), pPosition, (uint) sizeof(Vector3));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FullscreenVertex
        {
            public Vector2 Position;
            public Vector2 UV;
        }
    }
}
