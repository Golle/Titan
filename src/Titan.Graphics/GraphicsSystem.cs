using System;
using System.Numerics;
using Titan.Core;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Windows.D3D11;
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
        private const uint CameraSlot = 0u;

        private readonly Pipeline[] _pipeline;
        private readonly Context _immediateContext = GraphicsDevice.ImmediateContext;
        private readonly SwapChain _swapchain = GraphicsDevice.SwapChain;
        private readonly Handle<Buffer> _cameraBufferHandle;
        private readonly ViewPort _viewport;
        private Matrix4x4 _view;
        private Matrix4x4 _viewProject;

        public unsafe GraphicsSystem(Pipeline[] pipeline)
        {
            _pipeline = pipeline;

            _cameraBufferHandle = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Type = BufferTypes.ConstantBuffer,
                Count = 1,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint)sizeof(CameraBuffer),
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });

            _viewport = new ViewPort((int)GraphicsDevice.SwapChain.Width, (int)GraphicsDevice.SwapChain.Height);
        }

        public void SetCamera(in Matrix4x4 view, in Matrix4x4 viewProjection)
        {
            _view = view;
            _viewProject = viewProjection;
        }
        public void Render()
        {
            // set up camera
            //Matrix4x4.Transpose(cam.ViewProjection)
            _immediateContext.Map(_cameraBufferHandle, new CameraBuffer {View = _view, ViewProjection = Matrix4x4.Transpose(_viewProject)});
            
            _immediateContext.SetVertexShaderConstantBuffer(_cameraBufferHandle, CameraSlot);
            
            _immediateContext.SetViewPort(_viewport); // change this if we want to support more than a single viewport
            //execute pipeline
            foreach (ref readonly var pipeline in _pipeline.AsSpan())
            {
                if (pipeline.ClearRenderTargets)
                {
                    foreach (var handle in pipeline.RenderTargets)
                    {
                        _immediateContext.ClearRenderTarget(handle, pipeline.ClearColor);
                    }
                }

                if (pipeline.ClearDepthBuffer)
                {
                    _immediateContext.ClearDepthBuffer(pipeline.DepthBuffer, pipeline.DepthBufferClearValue);
                }
                
                _immediateContext.SetRenderTargets(pipeline.RenderTargets, pipeline.DepthBuffer);
                _immediateContext.SetPixelShaderSamplers(pipeline.PixelShaderSamplers);
                _immediateContext.SetVertexShaderSamplers(pipeline.VertexShaderSamplers);
                _immediateContext.SetPixelShaderResources(pipeline.PixelShaderResources);
                _immediateContext.SetVertexShaderResources(pipeline.VertexShaderResources);
                if (pipeline.VertexShader.IsValid())
                {
                    _immediateContext.SetVertexShader(pipeline.VertexShader);
                }

                if (pipeline.PixelShader.IsValid())
                {
                    _immediateContext.SetPixelShader(pipeline.PixelShader);
                }

                pipeline.Renderer.Render(_immediateContext);
                
                _immediateContext.UnbindRenderTargets();
                _immediateContext.UnbindPixelShaderResources(pipeline.PixelShaderResources);
                _immediateContext.UnbindVertexShaderResources(pipeline.VertexShaderResources);
            }

            // swapchain
            _swapchain.Present();
            //Thread.Sleep(100);
        }

        public void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_cameraBufferHandle);
        }
    }
}
