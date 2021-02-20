using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Rendering.Builder;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.GraphicsV2.Rendering
{
    internal enum RenderTextureFormat : uint
    {
        None,
        RG32F = DXGI_FORMAT_R32G32_FLOAT,
        RGB32F = DXGI_FORMAT_R32G32B32_FLOAT,
        RGBA32F = DXGI_FORMAT_R32G32B32A32_FLOAT,
    }

    internal enum DepthTextureFormat : uint
    {
        None,
        D16 = DXGI_FORMAT_D16_UNORM,
        D24S8 = DXGI_FORMAT_D24_UNORM_S8_UINT,
        D32 = DXGI_FORMAT_D32_FLOAT,
        D32S8X24 = DXGI_FORMAT_D32_FLOAT_S8X24_UINT
    }

    internal enum RenderStages
    {
        Geometry, 
        PostProcessing,
        Swapchain
    }

    internal enum RenderInputTypes
    {
        PixelShader,
        VertexShader
    }

    internal record RenderPipelineSpecification(string Name, TextureSpecification[] Textures, RenderStageSpecification[] Stages, DepthBufferSpecification[] DepthBuffers);

    internal record TextureSpecification(string Name, RenderTextureFormat Format);

    internal record DepthBufferSpecification(string Name, DepthTextureFormat Format);

    internal record RenderStageSpecification(string Name, RenderStages Stage, string Depth, RenderOutputSpecification Output, RenderInputSpecificaiton[] Inputs, string Material);

    internal record RenderOutputSpecification(string[] RenderTargets, bool Clear, string ClearColor, bool ClearDepth, float ClearDepthValue);

    internal record RenderInputSpecificaiton(string Name, RenderInputTypes Type);


    internal class RenderPipelineFactory
    {
        private readonly RenderPipelineReader _reader;
        private readonly Device _device;

        public RenderPipelineFactory(RenderPipelineReader reader, Device device)
        {
            _reader = reader;
            _device = device;
        }

        internal RenderPipeline CreateFromFile(string identifier)
        {
            var config = _reader.Read(identifier).First(); //TODO: add support for multiple pipelines

            var framebuffers = new Dictionary<string, Handle<Texture>>();
            foreach (var textureSpecification in config.Textures)
            {
                var handle = _device.TextureManager.Create(new TextureCreation
                {
                    Binding = TextureBindFlags.RenderTarget | TextureBindFlags.ShaderResource,
                    Format = (DXGI_FORMAT) textureSpecification.Format
                });
                framebuffers.Add(textureSpecification.Name, handle);
            }

            var stages = config.Stages.Select(stage =>
            {
                var builder = new RenderStageBuilder(stage.Name, framebuffers);
                foreach (var (name, type) in stage.Inputs)
                {
                    builder.AddInput(name, type);
                }

                var output = stage.Output;
                if (output != null)
                {
                    foreach (var renderTarget in output.RenderTargets)
                    {
                        builder.AddOutput(renderTarget);
                    }

                    if (output.Clear)
                    {
                        builder.ClearRenderTargets(Color.Parse(output.ClearColor));
                    }
                }
                return builder.Build(_device);
            }).ToArray();
            
            return new RenderPipeline(config.Name, stages);
        }

    }
}

