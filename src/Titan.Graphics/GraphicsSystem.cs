using System;
using System.Numerics;
using Titan.Core;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Windows.D3D11;

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
        private const uint OrthographicCameraSlot = 1u;

        private Pipeline[] _pipeline = Array.Empty<Pipeline>();
        private readonly Handle<ResourceBuffer> _cameraBufferHandle;
        private readonly Handle<ResourceBuffer> _orthographicCameraHandle;
        private ViewPort _viewport;
        private Matrix4x4 _view;
        private Matrix4x4 _viewProject;
        private readonly Matrix4x4 _orthographicCamera;

        public unsafe GraphicsSystem()
        {
            _cameraBufferHandle = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Type = BufferTypes.ConstantBuffer,
                Count = 1,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint)sizeof(CameraBuffer),
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });

            _orthographicCameraHandle = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Type = BufferTypes.ConstantBuffer,
                Count = 1,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint)sizeof(Matrix4x4),
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });

            var width = (int)GraphicsDevice.SwapChain.Width;
            var height = (int)GraphicsDevice.SwapChain.Height;
            _viewport = new ViewPort(width, height);
            _orthographicCamera = Matrix4x4.Transpose(Matrix4x4.CreateOrthographicOffCenter(0, width, 0, height, -1, 1));
        }

        public void SetCamera(in Matrix4x4 view, in Matrix4x4 viewProjection)
        {
            _view = view;
            _viewProject = Matrix4x4.Transpose(viewProjection);
        }

        public void Render()
        {
            // set up camera
            //Matrix4x4.Transpose(cam.ViewProjection)
            var context = GraphicsDevice.ImmediateContext;
            context.Map(_cameraBufferHandle, new CameraBuffer {View = _view, ViewProjection = _viewProject});
            context.Map(_orthographicCameraHandle, _orthographicCamera);

            context.SetVertexShaderConstantBuffer(_cameraBufferHandle, CameraSlot);
            context.SetVertexShaderConstantBuffer(_orthographicCameraHandle, OrthographicCameraSlot);

            context.SetViewPort(_viewport); // change this if we want to support more than a single viewport
            //execute pipeline
            foreach (ref readonly var pipeline in _pipeline.AsSpan())
            {
                var renderer = pipeline.Renderer;

                if (pipeline.ClearRenderTargets)
                {
                    foreach (var handle in pipeline.RenderTargets)
                    {
                        context.ClearRenderTarget(handle, pipeline.ClearColor);
                    }
                }

                if (pipeline.ClearDepthBuffer)
                {
                    context.ClearDepthBuffer(pipeline.DepthBuffer, pipeline.DepthBufferClearValue);
                }

                context.SetRenderTargets(pipeline.RenderTargets, pipeline.DepthBuffer);
                context.SetPixelShaderSamplers(pipeline.PixelShaderSamplers);
                context.SetVertexShaderSamplers(pipeline.VertexShaderSamplers);
                context.SetPixelShaderResources(pipeline.PixelShaderResources);
                context.SetVertexShaderResources(pipeline.VertexShaderResources);
                context.SetRasterizerState(pipeline.RasterizerState);
                if (pipeline.VertexShader.IsValid())
                {
                    context.SetVertexShader(pipeline.VertexShader);
                }

                if (pipeline.PixelShader.IsValid())
                {
                    context.SetPixelShader(pipeline.PixelShader);
                }

                context.SetBlendState(pipeline.BlendState);

                renderer.Render(context);
                
                
                context.UnbindRenderTargets();
                context.UnbindPixelShaderResources(pipeline.PixelShaderResources);
                context.UnbindVertexShaderResources(pipeline.VertexShaderResources);
            }

            // swapchain
            GraphicsDevice.SwapChain.Present();
            //Thread.Sleep(100);
        }

        public void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_cameraBufferHandle);
            GraphicsDevice.BufferManager.Release(_orthographicCameraHandle);
        }

        public void Init(Pipeline[] buildPipeline)
        {
            _pipeline = buildPipeline;
            var width = (int)GraphicsDevice.SwapChain.Width;
            var height = (int)GraphicsDevice.SwapChain.Height;
            _viewport = new ViewPort(width, height);
        }
    }
}
