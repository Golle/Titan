using System;
using System.Linq;
using Titan.GraphicsV2.D3D11;
using Titan.Windows.Win32.D3D11;
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
        public DepthStencilFormats DepthStencil { get; init; }
        public bool ClearDepthStencil { get; init; }
        public bool ClearRenderTargets { get; init; }
        public Color ClearColor { get; init; }
    }

    public record TextureSpecification(string Name, TextureFormats Format);

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


    public record RenderPassSpecification(string Name)
    {
        public VertexShaderSpecification VertexShader { get; init; }
        public PixelShaderSpecification PixelShader { get; init; }
    }

    public class RenderingPipeline
    {
        private FrameBufferFactory _frameBufferFactory;

        private RenderingPipeline(FrameBufferFactory frameBufferFactory)
        {
            _frameBufferFactory = frameBufferFactory;
        }

        public void Initialize()
        {
            // create framebuffers
            var geometryFrameBufferSpecification = new FrameBufferSpecification
            {
                DepthStencil = DepthStencilFormats.D24S8,
                Textures = new TextureSpecification[]
                {
                    new ("GBufferPosition", TextureFormats.RGBA32F),
                    new ("GBufferAlbedo", TextureFormats.RGBA32F),
                    new ("GBufferNormal", TextureFormats.RGBA32F),
                },
                ClearColor = Color.Black,
                ClearRenderTargets = true,
                Height =  1024,
                Width = 1024
            };
            var gbufferFrameBuffer = _frameBufferFactory.Create(geometryFrameBufferSpecification);


            var geometryBufferRenderPassSpecification = new RenderPassSpecification("GBuffer")
            {
                PixelShader = new PixelShaderSpecification("shaders/GBufferPixelShader.hlsl"),
                VertexShader = new VertexShaderSpecification("shaders/GBufferVertexShader.hlsl", new InputLayoutSpecification []
                {
                    new("Position", TextureFormats.RGB32F),
                    new("Normal", TextureFormats.RGB32F),
                    new("Texture", TextureFormats.RG32F)
                }),
                Sources = Array.Empty<string>(),
                Targets = new[] { "GBufferPosition", "GBufferAlbedo", "GBufferNormal" }
            };


            // create render passes that we support


            // Schedule the render passes for execution



        }
    }

    internal class FrameBuffer : IDisposable
    {
        private readonly RenderTargetView[] _renderTargets;
        private readonly ShaderResourceView[] _shaderResources;
        private readonly Texture2D[] _textures;

        
        private DepthStencilView _depthStencil;
        private bool _hasDepthStencil;

        public FrameBuffer(RenderTargetView[] renderTargets, ShaderResourceView[] shaderResources, Texture2D[] textures)
        {
            _renderTargets = renderTargets;
            _shaderResources = shaderResources;
            _textures = textures;
            
        }
        public FrameBuffer(RenderTargetView[] renderTargets, ShaderResourceView[] shaderResources, Texture2D[] textures, DepthStencilView depthStencil)
        {
            _renderTargets = renderTargets;
            _shaderResources = shaderResources;
            _textures = textures;
            _depthStencil = depthStencil;
            _hasDepthStencil = false;
        }


        internal ref readonly RenderTargetView GetTarget(int index) => ref _renderTargets[index];
        internal ref readonly ShaderResourceView GetSource(int index) => ref _shaderResources[index];



        public void Dispose()
        {
            foreach (var view in _renderTargets)
            {
                view.Release();
            }
            foreach (var resource in _shaderResources)
            {
                resource.Release();
            }
            foreach (var texture in _textures)
            {
                texture.Release();
            }
        }
    }
    internal class FrameBufferFactory
    {
        private readonly Texture2DFactory _texture2DFactory;
        private readonly ShaderResourceViewFactory _shaderResourceViewFactory;
        private readonly RenderTargetViewFactory _renderTargetViewFactory;
        private readonly DepthStencilViewFactory _depthStencilViewFactory;

        public FrameBufferFactory(Texture2DFactory texture2DFactory, ShaderResourceViewFactory shaderResourceViewFactory, RenderTargetViewFactory renderTargetViewFactory, DepthStencilViewFactory depthStencilViewFactory)
        {
            _texture2DFactory = texture2DFactory;
            _shaderResourceViewFactory = shaderResourceViewFactory;
            _renderTargetViewFactory = renderTargetViewFactory;
            _depthStencilViewFactory = depthStencilViewFactory;
        }
        
        internal FrameBuffer Create(FrameBufferSpecification specification)
        {
            var textures = specification.Textures
                .Select(t => _texture2DFactory.Create(specification.Width, specification.Height, (DXGI_FORMAT) t.Format, bindFlag: D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE))
                .ToArray();

            var renderTargets = textures.Select(t => _renderTargetViewFactory.Create(t)).ToArray();
            var shaderResources = textures.Select(t => _shaderResourceViewFactory.Create(t)).ToArray();

            if (specification.DepthStencil == DepthStencilFormats.None)
            {
                return new FrameBuffer(renderTargets, shaderResources, textures);
            }


            if(specification.DepthStencil != DepthStencilFormats.None)
            {
                var textureFormat = specification.DepthStencil switch
                {
                    DepthStencilFormats.D16 => DXGI_FORMAT_R16_TYPELESS,
                    DepthStencilFormats.D24S8 => DXGI_FORMAT_R24G8_TYPELESS,
                    _ => throw new NotSupportedException($"Format: {specification.DepthStencil} is not supported.")
                };

                var texture = _texture2DFactory.Create(specification.Width, specification.Height, textureFormat, bindFlag: D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
                unsafe
                {

                    var depthStencilView = _depthStencilViewFactory.Create(texture, (DXGI_FORMAT) specification.DepthStencil);
                }
            }
            

            return null;
        }
    }
}
