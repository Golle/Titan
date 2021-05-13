using System;
using System.Numerics;
using Titan.Core;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Textures;
using Buffer = Titan.Graphics.D3D11.Buffers.Buffer;

namespace Titan.Graphics
{

    internal struct CameraBuffer
    {
        public Matrix4x4 View;
        public Matrix4x4 ViewProjection;
    }

    public class GraphicsSystem : IDisposable
    {
        private readonly Pipeline[] _pipeline;
        private readonly Context _immediateContext = GraphicsDevice.ImmediateContext;
        private readonly SwapChain _swapchain = GraphicsDevice.SwapChain;
        private readonly TextureManager _textureManager = GraphicsDevice.TextureManager;
        private readonly Handle<Buffer> _cameraBufferHandle;

        public unsafe GraphicsSystem(Pipeline[] pipeline)
        {
            _pipeline = pipeline;

            //_cameraBufferHandle = GraphicsDevice.BufferManager.Create(new BufferCreation
            //{
            //    Type = BufferTypes.ConstantBuffer,
            //    Count = 1,
            //    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
            //    Stride = (uint) sizeof(CameraBuffer),
            //    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            //});
        }

        public void Render()
        {
            // set up camera
            //_immediateContext.Map(_cameraBufferHandle, new CameraBuffer {View = Matrix4x4.Identity, ViewProjection = Matrix4x4.Identity});

            // execute pipeline
            //foreach (ref readonly var pipeline in _pipeline.AsSpan())
            //{
            //    if (pipeline.ClearRenderTargets)
            //    {
            //        foreach (var handle in pipeline.RenderTargets)
            //        {
            //            _immediateContext.ClearRenderTarget(handle, pipeline.ClearColor);
            //        }
            //    } 
                



            //}


            // swapchain
            _swapchain.Present();

        }


        public void Dispose()
        {

        }
    }
}
