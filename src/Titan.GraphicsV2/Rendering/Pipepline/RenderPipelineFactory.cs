using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core.Common;
using Titan.Core.IO;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Samplers;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Rendering.Builder;
using Titan.GraphicsV2.Rendering.Renderers;
using Titan.IOC;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Pipepline
{
    internal record RenderPipelineSpecification(string Name, TextureSpecification[] Textures, RenderStageSpecification[] Stages, DepthBufferSpecification[] DepthBuffers, SamplerSpecification[] Samplers, ShaderSpecification[] Shaders, RendererSpecification[] Renderers);
    internal record RendererSpecification(string Name, string Type);
    internal record TextureSpecification(string Name, RenderTextureFormat Format);
    internal record DepthBufferSpecification(string Name, DepthTextureFormat Format);
    internal record RenderStageSpecification(string Name, string Renderer, string Depth, RenderOutputSpecification Output, RenderInputSpecificaiton Input, string Material);
    internal record RenderOutputSpecification(string[] RenderTargets, bool Clear, string ClearColor, bool ClearDepth, float ClearDepthValue);
    internal record RenderInputSpecificaiton(RenderBinding[] Textures, RenderBinding[] Samplers);
    internal record RenderBinding(string Name, RenderBindingTypes Type);
    internal record SamplerSpecification(string Name, TextureFilter Filter, TextureAddressMode AddressU, TextureAddressMode AddressV, TextureAddressMode AddressW, ComparisonFunc ComparisonFunc);
    internal record ShaderSpecification(string Name, VertexShaderSpecification VertexShader, PixelShaderSpecification PixelShader);
    internal record VertexShaderSpecification(string Path, InputLayoutSpecification[] Layout, string Entrypoint = "main", string Version = "vs_5_0");
    internal record InputLayoutSpecification(string Name, TextureFormats Format);
    internal record PixelShaderSpecification(string Path, string Entrypoint = "main", string Version = "ps_5_0");

    internal class RenderPipelineFactory
    {
        private readonly RenderPipelineReader _reader;
        private readonly IFileReader _fileReader;
        private readonly Device _device;
        private readonly IContainer _container;

        public RenderPipelineFactory(RenderPipelineReader reader, IFileReader fileReader, Device device, IContainer container)
        {
            _reader = reader;
            _fileReader = fileReader;
            _device = device;
            _container = container;
        }

        internal RenderPipeline CreateFromFile(string identifier)
        {
            var config = _reader.Read(identifier).First(); //TODO: add support for multiple pipelines

            var framebuffers = CreateFramebuffers(config);
            var shaders = CreateShaders(config);
            var samplers = CreateSamplers(config);
            var renderers = config.Renderers.ToDictionary(r => r.Name, r => r.Type);
            

            var stages = config.Stages.Select(stage =>
            {
                // TODO: this will create a new renderer for each stage. since renderers are stateless, we could re-use them. How do we handle the lifetime of a renderer? new instance = handled inside the RenderStage, shared instance = handled where?
                var rendererType = Type.GetType(renderers[stage.Renderer]);
                if(rendererType == null)
                {
                    throw new InvalidOperationException($"Failed to get the type for the renderer: {stage.Renderer}");
                }
                var renderer = (IRenderer)_container.CreateInstance(rendererType);

                var builder = new RenderStageBuilder(stage.Name, renderer, framebuffers, samplers, shaders);
                foreach (var texture in stage.Input?.Textures ?? Enumerable.Empty<RenderBinding>())
                {
                    builder.AddInput(texture.Name, texture.Type);
                }

                foreach (var sampler in stage.Input?.Samplers ?? Enumerable.Empty<RenderBinding>())
                {
                    builder.AddSampler(sampler.Name, sampler.Type);
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

                if (!string.IsNullOrWhiteSpace(stage.Material))
                {
                    builder.UseShader(stage.Material);
                }

                return builder.Build();
            }).ToArray();
            
            return new RenderPipeline(config.Name, stages);
        }
       
        private Dictionary<string, Sampler> CreateSamplers(RenderPipelineSpecification config)
        {
            var samplers = new Dictionary<string, Sampler>();
            foreach (var spec in config.Samplers)
            {
                var handle = _device.SamplerManager.Create(new SamplerCreation
                {
                    AddressU = spec.AddressU,
                    AddressW = spec.AddressW,
                    AddressV = spec.AddressV,
                    ComparisonFunc = spec.ComparisonFunc,
                    Filter = spec.Filter
                });
                samplers.Add(spec.Name, _device.SamplerManager.Access(handle));
            }

            return samplers;
        }

        private Dictionary<string, Texture> CreateFramebuffers(RenderPipelineSpecification config)
        {
            var framebuffers = new Dictionary<string, Texture>();
            foreach (var textureSpecification in config.Textures)
            {
                var handle = _device.TextureManager.Create(new TextureCreation
                {
                    Binding = TextureBindFlags.RenderTarget | TextureBindFlags.ShaderResource,
                    Format = (DXGI_FORMAT) textureSpecification.Format
                });
                var texture = _device.TextureManager.Access(handle);
                framebuffers.Add(textureSpecification.Name, texture);
            }

            // Add the Backbuffer to the framebuffers
            unsafe
            {
                framebuffers.Add("$Backbuffer", new Texture
                {
                    D3DTarget = _device.Swapchain.Backbuffer
                });
            }
            
            return framebuffers;
        }

        private Dictionary<string, Shader> CreateShaders(RenderPipelineSpecification config)
        {
            var shaders = new Dictionary<string, Shader>();
            foreach (var (name, vertexShader, pixelShader) in config.Shaders)
            {
                // TOOD: this might not be the best way to do this
                var pixelshaderSource = _fileReader.ReadText(pixelShader.Path);
                var vertexShaderSource = _fileReader.ReadText(vertexShader.Path);
                var handle = _device.ShaderManager.Create(new ShaderCreation
                {
                    InputLayout = vertexShader.Layout.Select(i => new InputLayoutDescription(i.Name, i.Format)).ToArray(),
                    PixelShader = new ShaderDescription(pixelshaderSource, pixelShader.Entrypoint, pixelShader.Version),
                    VertexShader = new ShaderDescription(vertexShaderSource, vertexShader.Entrypoint, vertexShader.Version)
                });
                shaders.Add(name, _device.ShaderManager.Access(handle));
            }

            return shaders;
        }
    }
}

