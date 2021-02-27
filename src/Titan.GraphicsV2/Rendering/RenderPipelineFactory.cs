using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.IO;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Samplers;
using Titan.GraphicsV2.D3D11.Shaders;
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

    internal enum RenderBindingTypes
    {
        ComputeShader,
        PixelShader,
        VertexShader
    }

    internal record RenderPipelineSpecification(string Name, TextureSpecification[] Textures, RenderStageSpecification[] Stages, DepthBufferSpecification[] DepthBuffers, SamplerSpecification[] Samplers, ShaderSpecification[] Shaders);
    internal record TextureSpecification(string Name, RenderTextureFormat Format);
    internal record DepthBufferSpecification(string Name, DepthTextureFormat Format);
    internal record RenderStageSpecification(string Name, RenderStages Stage, string Depth, RenderOutputSpecification Output, RenderInputSpecificaiton Input, string Material);
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

        public RenderPipelineFactory(RenderPipelineReader reader, IFileReader fileReader, Device device)
        {
            _reader = reader;
            _fileReader = fileReader;
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

            var shaders = new Dictionary<string, Handle<Shader>>();
            foreach (var (name, vertexShader, pixelShader) in config.Shaders)
            {
                // TOOD: this might not be the best way to do this
                var pixelshaderSource = _fileReader.ReadText(pixelShader.Path);
                var vertexShaderSource = _fileReader.ReadText(vertexShader.Path);
                var handle = _device.ShaderManager.Create(new ShaderCreation
                {
                    InputLayout = vertexShader.Layout.Select(i => new InputLayoutDescription(i.Name, i.Format)).ToArray(),
                    PixelShader = new ShaderDescription(pixelshaderSource, pixelShader.Entrypoint, pixelShader.Version),
                    VertexShader = new ShaderDescription(vertexShaderSource, pixelShader.Entrypoint, vertexShader.Version)
                });
                shaders.Add(name, handle);
            }

            var samplers = new Dictionary<string, Handle<Sampler>>();
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
                samplers.Add(spec.Name, handle);
            }

            var stages = config.Stages.Select(stage =>
            {
                var builder = new RenderStageBuilder(stage.Name, stage.Stage, framebuffers, samplers, shaders);
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

                return builder.Build(_device);
            }).ToArray();
            
            return new RenderPipeline(config.Name, stages);
        }

    }
}

