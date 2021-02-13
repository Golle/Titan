using System;
using System.Collections.Generic;
using Titan.GraphicsV2.D3D11;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.GraphicsV2.Rendering
{

    public enum TextureFormats : uint
    {
        None,
        RG32F = DXGI_FORMAT_R32G32_FLOAT,
        RGB32F = DXGI_FORMAT_R32G32B32_FLOAT,
        RGBA32F = DXGI_FORMAT_R32G32B32A32_FLOAT,
    }

    public enum DepthStencilFormats : uint
    {
        None,
        D16 = DXGI_FORMAT_D16_UNORM,
        D24S8 = DXGI_FORMAT_D24_UNORM_S8_UINT,
        D32 = DXGI_FORMAT_D32_FLOAT,
        D32S8X24 = DXGI_FORMAT_D32_FLOAT_S8X24_UINT
    }

    public record FrameBufferSpecification
    {
        public uint Width { get; init; }
        public uint Height { get; init; }
        public TextureSpecification[] Textures { get; init; }
        public DepthStencilSpecification DepthStencil { get; init; }
    }

    public record DepthStencilSpecification(string Name, DepthStencilFormats Format, bool Clear = false, Color ClearColor = default);

    public record TextureSpecification(string Name, TextureFormats Format, bool Clear = false, Color ClearColor = default);

    public record InputLayoutSpecification(string SemanticName, TextureFormats Format);

    public record PixelShaderSpecification(string Filename)
    {
        public string Entrypoint { get; init; } = "main";
        public string Version { get; init; } = "ps_5_0";
    }

    public record VertexShaderSpecification(string Filename, InputLayoutSpecification[] InputLayout)
    {
        public string Entrypoint { get; init; } = "main";
        public string Version { get; init; } = "vs_5_0";
    }


    internal record RenderPassSpecification(string Name)
    {
        public VertexShaderSpecification VertexShader { get; init; }
        public PixelShaderSpecification PixelShader { get; init; }
        public string[] Sources { get; init; } = Array.Empty<string>();
        public string[] Targets { get; init; } = Array.Empty<string>();

    }

    internal record PipelineConfiguration
    {
        internal RenderPassSpecification[] RenderPasses { get; init; } = Array.Empty<RenderPassSpecification>();
        internal FrameBufferSpecification[] FrameBuffers { get; init; } = Array.Empty<FrameBufferSpecification>();
    }

    internal class RenderingPipeline
    {
        private readonly RenderPipelineFactory _renderPipelineFactory;

        internal RenderingPipeline(RenderPipelineFactory renderPipelineFactory)
        {
            _renderPipelineFactory = renderPipelineFactory;
        }

        public RenderPass[] Initialize()
        {
            // Special name to allow backbuffer creation
            var backbuffer = new FrameBufferSpecification
            {
                Textures = new[] {new TextureSpecification("$Backbuffer", TextureFormats.RGBA32F, true, Color.Green)},
                DepthStencil = new DepthStencilSpecification("DefaultDepthStencil", DepthStencilFormats.D24S8),
            };

            // create framebuffers
            var gbufferSpec = new FrameBufferSpecification
            {
                
                Textures = new TextureSpecification[]
                {
                    new ("GBufferPosition", TextureFormats.RGBA32F, true, Color.Black),
                    new ("GBufferAlbedo", TextureFormats.RGBA32F, true, Color.Black),
                    new ("GBufferNormal", TextureFormats.RGBA32F, true, Color.Black),
                },
                Height =  0, 
                Width = 0
            };
            
            var gbufferRenderPass = new RenderPassSpecification("GBuffer")
            {
                PixelShader = new PixelShaderSpecification("shaders/GBufferPixelShader.hlsl"),
                VertexShader = new VertexShaderSpecification("shaders/GBufferVertexShader.hlsl", new InputLayoutSpecification []
                {
                    new("Position", TextureFormats.RGB32F),
                    new("Normal", TextureFormats.RGB32F),
                    new("Texture", TextureFormats.RG32F)
                }),
                Targets = new[] { "GBufferPosition", "GBufferAlbedo", "GBufferNormal" },
                //Renderer = "SceneRenderer"
            };

            var backbufferRenderPass = new RenderPassSpecification("Backbuffer")
            {
                PixelShader = new PixelShaderSpecification("shaders/BackbufferPixelShader.hlsl"),
                VertexShader = new VertexShaderSpecification("shaders/BackbufferVertexShader.hlsl", new []
                {
                    new InputLayoutSpecification("Position", TextureFormats.RG32F), 
                    new InputLayoutSpecification("Texture", TextureFormats.RG32F)
                }),
                Sources = new[] {"GBufferAlbedo"},
                Targets = new[] {"$Backbuffer"},
                //Renderer = "FullscreenRenderer"
            };

            // create render passes that we support

            var config = new PipelineConfiguration
            {
                FrameBuffers = new[] {backbuffer, gbufferSpec},
                RenderPasses = new[] {gbufferRenderPass, backbufferRenderPass}
            };

            return _renderPipelineFactory.CreatePipeline(config);

            // Schedule the render passes for execution


        }
    }
}
