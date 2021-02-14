using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.GraphicsV2.Rendering2
{
    internal enum RenderTextureFormat : uint
    {
        None,
        
        // Texture formats
        RG32F = DXGI_FORMAT_R32G32_FLOAT,
        RGB32F = DXGI_FORMAT_R32G32B32_FLOAT,
        RGBA32F = DXGI_FORMAT_R32G32B32A32_FLOAT,

        // Depth formats
        // TODO: separete these?
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

    internal record RenderStageSpecification(string Name, RendersStages Stage, string Depth, RenderOutputSpecification Output, RenderInputSpecificaiton[] Inputs, string Material);

    internal record RenderOutputSpecification(string[] RenderTargets, bool Clear, string ClearColor, bool ClearDepth, float ClearDepthValue);

    internal record RenderInputSpecificaiton(string Name, object Type);
}

