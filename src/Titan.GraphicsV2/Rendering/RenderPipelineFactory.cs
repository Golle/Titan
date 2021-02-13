using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.Resources;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering
{
    internal unsafe class RenderPipelineFactory
    {
        private readonly FrameBufferFactory _frameBufferFactory;
        private readonly RenderPassFactory _renderPassFactory;

        public RenderPipelineFactory(FrameBufferFactory frameBufferFactory, RenderPassFactory renderPassFactory)
        {
            _frameBufferFactory = frameBufferFactory;
            _renderPassFactory = renderPassFactory;
        }

        public RenderPass[] CreatePipeline(PipelineConfiguration configuration)
        {
            
            var frameBuffers = configuration
                .FrameBuffers
                .Select(f => _frameBufferFactory.Create(f))
                .ToArray();
            
            var passes = configuration
                .RenderPasses
                .Select(r => _renderPassFactory.Create(r, frameBuffers))
                .ToArray();

            return passes;
        }
    }

    internal class RenderPassFactory
    {
        private readonly ShaderManager _shaderManager;

        public RenderPassFactory(ShaderManager shaderManager, Device device)
        {
            _shaderManager = shaderManager;
        }

        internal unsafe RenderPass Create(RenderPassSpecification specification, in FrameBuffer[] frameBuffers)
        {
            //TODO: These can be null
            var pixelshader = _shaderManager.Load(specification.PixelShader); 
            var vertexShader = _shaderManager.Load(specification.VertexShader);

            var frameBufferTextures = frameBuffers
                .SelectMany(f => f.Textures)
                .ToDictionary(t => t.Name);


            var builder = new CommandListBuilder();
            var renderTargets = stackalloc ID3D11RenderTargetView*[specification.Targets.Length];
            ID3D11DepthStencilView* depthStencil = ;
            for (var i = 0u; i < specification.Targets.Length; ++i)
            {
                var target =  frameBufferTextures[specification.Targets[i]];
                if (target.Clear)
                {
                    builder.AddClearRenderTarget(target.RenderTargetView, target.Color);
                }
                renderTargets[i] = target.RenderTargetView;
                
            }

            builder.SetRenderTargets(specification.Targets.Length, renderTargets, );

            return new RenderPass(builder.Build());
        }
    }

 }
