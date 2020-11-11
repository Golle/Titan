using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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


        private readonly VertexBufferHandle _vertexBufferHandle;
        private readonly IndexBufferHandle _indexBufferHandle;
        private readonly ConstantBufferHandle _lightSourceHandle;
        private readonly ShaderProgram _shader;

        

        public unsafe DefaultLightsRenderer(IGraphicsDevice device, ILigthRenderQueue ligthRenderQueue)
        {
            _ligthRenderQueue = ligthRenderQueue;
            _vertexBufferManager = device.VertexBufferManager;
            _indexBufferManager = device.IndexBufferManager;
            _constantBufferManager = device.ConstantBufferManager;
            _shaderManager = device.ShaderManager;

            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), UV = new Vector2(0, 1) };
            vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), UV = new Vector2(0, 0) };
            vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), UV = new Vector2(1, 0) };
            vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), UV = new Vector2(1, 1) };
            _vertexBufferHandle = _vertexBufferManager.CreateVertexBuffer(4u, (uint)sizeof(FullscreenVertex), vertices);
            
            var indices = stackalloc ushort[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            _indexBufferHandle = _indexBufferManager.CreateIndexBuffer<ushort>(6, indices);
            _lightSourceHandle = _constantBufferManager.CreateConstantBuffer(new LightSource
            {
                Position = new Vector3(0, 0, -1)
            }, D3D11_USAGE.D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);


            _shader = device.ShaderManager.GetByName("DeferredShadingDefault");
        }

        public void Render(IRenderContext context)
        {
            ref readonly var lightSource = ref _constantBufferManager[_lightSourceHandle];
            unsafe
            {
                var lights = _ligthRenderQueue.GetLights();
                var light = new Lights {_numberOfLigts = 1};
                fixed (Light* l = lights)
                {
                    Unsafe.CopyBlock(light.LightPositions, (float*)l, (uint)(lights.Length * sizeof(Light)));
                }
                
                for (var i = 0; i < lights.Length; ++i)
                {
                    light.Set(i, lights[i].Position);
                }
                
                context.MapResource(lightSource.Resource, light);
            }
            
            context.SetPixelShaderConstantBuffer(lightSource);

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBufferManager[_vertexBufferHandle]);
            context.SetIndexBuffer(_indexBufferManager[_indexBufferHandle]);

            context.SetInputLayout(_shaderManager[_shader.InputLayout]);
            context.SetVertexShader(_shaderManager[_shader.VertexShader]);
            context.SetPixelShader(_shaderManager[_shader.PixelShader]);

            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            _constantBufferManager.DestroyBuffer(_lightSourceHandle);
            _vertexBufferManager.DestroyBuffer(_vertexBufferHandle);
            _indexBufferManager.DestroyBuffer(_indexBufferHandle);
        }

        private unsafe struct Lights
        {
            public int _numberOfLigts;
            public fixed float LightPositions[32 * 3];

            public void Set(int index, in Vector3 position)
            {
                fixed (Vector3* pPosition = &position)
                fixed(float* pLights = LightPositions)
                {
                    Unsafe.CopyBlock(pPosition,pLights+(index*3), sizeof(float)*3);
                }

                //var i = index * 3;
                //LightPositions[i] = position.X;
                //LightPositions[i+1] = position.Z;
                //LightPositions[i+2] = position.X;
            }
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
