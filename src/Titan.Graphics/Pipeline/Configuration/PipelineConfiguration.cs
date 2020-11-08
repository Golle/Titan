using Titan.Graphics.D3D11;
using Titan.Graphics.Shaders1;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Configuration
{
    public record PipelineConfiguration(ShaderProgramConfiguration[] ShaderPrograms, RenderPassConfiguration[] RenderPasses, RendererConfiguration[] Renderers, SamplerConfiguration[] Samplers);
    public record SamplerConfiguration(string Name, D3D11_FILTER Filter, D3D11_TEXTURE_ADDRESS_MODE AddressU, D3D11_TEXTURE_ADDRESS_MODE AddressV, D3D11_TEXTURE_ADDRESS_MODE AddressW, D3D11_COMPARISON_FUNC ComparisonFunc);
    public record RendererConfiguration(string Name, string Type);
    public record RenderPassConfiguration(string Name, string Renderer, DepthStencilConfiguration DepthStencil, RenderTargetConfiguration[] RenderTargets, RenderPassResourceConfiguration[] Resources, RenderPassSamplerConfiguration[] Samplers, bool UnbindRenderTargets, bool UnbindResources);
    public record DepthStencilConfiguration(string Name); // TODO: add format etc for this later
    public record RenderPassResourceConfiguration(string Name, RenderPassResourceTypes Type);
    public record RenderPassSamplerConfiguration(string Name, SamplerType Type);
    public record RenderTargetConfiguration(string Name, DXGI_FORMAT Format, bool Clear, string Color)
    {
        public bool IsGlobal() => Name.StartsWith('$');
    }
    public record ShaderProgramConfiguration(string Name, VertexShaderDescriptor VertexShader, PixelShaderDescriptor PixelShader, InputLayoutDescriptor[] Layout);

    public enum RenderPassResourceTypes
    {
        VertexShader,
        PixelShader
    }

    public enum SamplerType
    {
        VertexShader,
        PixelShader
    }
}
