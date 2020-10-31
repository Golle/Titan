using Titan.Graphics.D3D11;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Configuration
{
    public record PipelineConfiguration(ShaderProgramConfiguration[] ShaderPrograms, RenderPassConfiguration[] RenderPasses);
    public record RenderPassConfiguration(string Name, string Renderer, object DepthStencil, RenderTargetConfiguration[] RenderTargets);
    public record RenderTargetConfiguration(string Name, DXGI_FORMAT Format);
    public record ShaderProgramConfiguration(string Name, VertexShaderDescriptor VertexShader, PixelShaderDescriptor PixelShader, InputLayoutDescriptor[] Layout);
}
