using System.Numerics;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.GraphicsV2.D3D11.Samplers;
using Titan.GraphicsV2.Rendering.Queue;
using Titan.Windows.Win32.D3D11;
using Buffer = Titan.GraphicsV2.D3D11.Buffers.Buffer;

namespace Titan.GraphicsV2.Rendering.Renderers
{

    internal struct CameraBuffer
    {
        public Matrix4x4 View;
        public Matrix4x4 ViewProjection;
    }

    internal class GeometryRenderer : IRenderer
    {
        private readonly Device _device;
        private readonly ModelRenderQueue _queue;
        private ViewPort _viewPort;
        
        private Buffer _cameraBuffer;
        private Buffer _worldBuffer;
        private Sampler _tempSampler;

        public GeometryRenderer(Device device, ModelRenderQueue queue)
        {
            _device = device;
            _queue = queue;
        }

        public void Init()
        {
            _viewPort = new ViewPort((int) _device.Swapchain.Width, (int) _device.Swapchain.Height);
            
            unsafe
            {
                var handle = _device.BufferManager.Create(new BufferCreation
                {
                    Count = 1,
                    Stride = (uint) sizeof(CameraBuffer),
                    Type = BufferTypes.ConstantBuffer,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                });
                _cameraBuffer = _device.BufferManager.Access(handle);
            }

            unsafe
            {
                var handle = _device.BufferManager.Create(new BufferCreation
                {
                    Count = 1,
                    Stride = (uint)sizeof(Matrix4x4),
                    Type = BufferTypes.ConstantBuffer,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                });
                _worldBuffer = _device.BufferManager.Access(handle);
            }

            _tempSampler = _device.SamplerManager.Access(_device.SamplerManager.Create(new SamplerCreation
            {
                AddressU = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                Filter = TextureFilter.MinMagMipLinear
            }));
        }


        public void Render(Context context)
        {
            context.Map(_cameraBuffer, new CameraBuffer { View = _queue.View, ViewProjection = Matrix4x4.Transpose(_queue.ViewProjection) });
            context.SetViewPort(_viewPort);
            context.SetPixelShaderSampler(_tempSampler);
            
            context.SetVSConstantBuffer(_cameraBuffer);
            context.SetVSConstantBuffer(_worldBuffer, 1);
            //unsafe
            //{
            //    var constantBuffers = stackalloc ID3D11Buffer*[2];
                
            //}
            

            foreach (ref readonly var renderable in _queue.GetRendereables())
            {
                ref readonly var world = ref renderable.World;
                ref readonly var model = ref renderable.Model;

                context.Map(_worldBuffer, world);
                

                context.SetTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
                context.SetVertexBuffer(_device.BufferManager.Access(model.VertexBuffer));
                ref readonly var indexBuffer = ref _device.BufferManager.Access(model.IndexBuffer);
                context.SetIndexBuffer(indexBuffer);
                if (model.SubMeshes.Length > 1)
                {
                    for (var i = 0; i < model.SubMeshes.Length; ++i)
                    {
                        ref readonly var submesh = ref model.SubMeshes[i];
                        if (submesh.HasMaterial && submesh.Material.DiffuseTexture.IsValid())
                        {
                            context.SetPixelShaderResource(_device.TextureManager.Access(submesh.Material.DiffuseTexture));
                        }
                        context.DrawIndexed(submesh.Count, submesh.Start);
                    }
                }
                else
                {
                    context.DrawIndexed(indexBuffer.Count);
                }
            }
            _queue.Reset();
        }

        public void Dispose()
        {
            _device.BufferManager.Release(_cameraBuffer.Handle);
            _device.BufferManager.Release(_worldBuffer.Handle);
            _cameraBuffer = default;
            _worldBuffer = default;
            _viewPort = default;
        }
    }
}
