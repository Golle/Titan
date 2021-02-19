using System.Linq;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Textures;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.GraphicsV2.Rendering2
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

    internal enum RendersStages
    {
        Geometry, 
        PostProcessing,
        Swapchain
    }

    internal record RenderPipelineSpecification(string Name, TextureSpecification[] Textures, RenderStageSpecification[] Stages);

    internal record TextureSpecification(string Name, RenderTextureFormat Format);

    internal record DepthBufferSpecification(string Name, DepthTextureFormat Format);

    internal record RenderStageSpecification(string Name, RendersStages Stage, string Depth, RenderOutputSpecification Output, RenderInputSpecificaiton[] Inputs, string Material);

    internal record RenderOutputSpecification(string[] RenderTargets, bool Clear, string ClearColor, bool ClearDepth, float ClearDepthValue);

    internal record RenderInputSpecificaiton(string Name, object Type);



    internal class Framebuffer
    {
        public string Name { get; }
        public RenderTextureFormat Format { get; }
        public Framebuffer(string name, RenderTextureFormat format)
        {
            Name = name;
            Format = format;
        }


        internal void Initialize(Device device)
        {
            var texture = device.TextureManager.Create(new TextureCreation());
        }
        internal void Dispose() { }

    }





    internal class RenderPipelineFactory
    {
        private readonly RenderPipelineReader _reader;

        public RenderPipelineFactory(RenderPipelineReader reader)
        {
            _reader = reader;
        }

        internal object CreateFromFile(string identifier)
        {
            var config = _reader.Read(identifier).First(); //TODO: add support for multiple pipelines

            //var textures = config.Textures.Select(t => CreateTexture(t));
            
            



            //config.Stages.Select(s =>
            //{
                

            //});



            return new RenderPipeline(config.Name);
        }

        //private object CreateTexture(TextureSpecification spec)
        //{
            
        //}
    }

    internal class RenderPipeline
    {
        public string Name { get; }
        public RenderPipeline(string name)
        {
            Name = name;
        }
    }
}

