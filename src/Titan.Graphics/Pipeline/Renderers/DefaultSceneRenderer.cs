using System;
using System.Numerics;
using Titan.Graphics.Camera;
using Titan.Graphics.D3D11;
using Titan.Graphics.Materials;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{

    
    internal class DefaultSceneRenderer : IRenderer
    {
        private readonly IMeshRenderQueue _renderQueue;
        private readonly ICameraManager _cameraManager;
        private readonly IMaterialsManager _materialsManager;
        private readonly IShaderManager _shaderManager;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;
        private readonly IConstantBufferManager _constantBufferManager;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;
        private readonly ISamplerStateManager _samplerStateManager;

        private readonly ConstantBufferHandle _perObjectHandle;
        private readonly ConstantBufferHandle _cameraHandle;

        private readonly SamplerStateHandle _samplerStatehandle;

        private readonly ShaderProgram _shader;

        public DefaultSceneRenderer(IGraphicsDevice device, IMeshRenderQueue renderQueue, ICameraManager cameraManager, IMaterialsManager materialsManager)
        {
            _renderQueue = renderQueue;
            _cameraManager = cameraManager;
            _materialsManager = materialsManager;
            _shaderManager = device.ShaderManager;
            _vertexBufferManager = device.VertexBufferManager;
            _indexBufferManager = device.IndexBufferManager;
            _constantBufferManager = device.ConstantBufferManager;
            _shaderResourceViewManager = device.ShaderResourceViewManager;
            _samplerStateManager = device.SamplerStateManager;
            
            _perObjectHandle = _constantBufferManager.CreateConstantBuffer<Matrix4x4>(usage: D3D11_USAGE.D3D11_USAGE_DYNAMIC, cpuAccess: D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);
            _cameraHandle = _constantBufferManager.CreateConstantBuffer<CameraBuffer>(usage: D3D11_USAGE.D3D11_USAGE_DYNAMIC, cpuAccess: D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);

            _samplerStatehandle = device.SamplerStateManager.GetOrCreate();
            _shader = _shaderManager.GetByName("GBufferDefault");
            //_shader = _shaderManager.GetByName("GBufferDefaultNormalMap");
        }

        private Vector2 modelRot = new Vector2(0, 0);
        
        public unsafe void Render(IRenderContext context)
        {
            var cam = _cameraManager.GetCamera();
            var camera = new CameraBuffer
            {
                View = cam.View,
                ViewProjection = Matrix4x4.Transpose(cam.ViewProjection)
            };

            context.SetInputLayout(_shaderManager[_shader.InputLayout]);
            context.SetVertexShader(_shaderManager[_shader.VertexShader]);
            context.SetPixelShader(_shaderManager[_shader.PixelShader]);

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            ref readonly var cameraBuffer = ref _constantBufferManager[_cameraHandle];
            context.MapResource(cameraBuffer.Resource, camera); // only needs to be done when the camera changes 
            
            context.SetVertexShaderConstantBuffer(cameraBuffer);
            context.SetPixelShaderSampler(_samplerStateManager[_samplerStatehandle]);

            foreach (ref readonly var renderable in _renderQueue.GetRenderables())
            {
                ref readonly var objectBuffer = ref _constantBufferManager[_perObjectHandle];
                context.MapResource(objectBuffer.Resource, renderable.World);
                context.SetVertexShaderConstantBuffer(objectBuffer, 1u);

                var mesh = renderable.Mesh;
                ref readonly var indexBuffer = ref _indexBufferManager[mesh.IndexBufferHandle];
                context.SetIndexBuffer(indexBuffer);
                context.SetVertexBuffer(_vertexBufferManager[mesh.VertexBufferHandle]);

                var subsets = mesh.SubSets;
                if (subsets.Length > 1)
                {
                    for (var i = 0; i < subsets.Length; ++i)
                    {
                        ref readonly var subset = ref subsets[i];
                        ref readonly var material = ref _materialsManager[renderable.Materials[subset.MaterialIndex]];
                        if (material.IsTextured)
                        {
                            context.SetPixelShaderResource(_shaderResourceViewManager[material.DiffuseMap.Handle]);
                        }
                        else
                        {
                            context.SetPixelShaderResource(_shaderResourceViewManager[renderable.Texture.ResourceViewHandle]);
                        }
                        context.DrawIndexed(subset.Count, subset.StartIndex);
                    }
                }
                else
                {
                    context.SetPixelShaderResource(_shaderResourceViewManager[renderable.Texture.ResourceViewHandle]);
                    context.DrawIndexed(indexBuffer.NumberOfIndices);
                }
            }
        }

        private struct CameraBuffer
        {
            public Matrix4x4 View;
            public Matrix4x4 ViewProjection;
        }

        public void Dispose()
        {
            _constantBufferManager.DestroyBuffer(_cameraHandle);
            _constantBufferManager.DestroyBuffer(_perObjectHandle);
        }
    }
}
