namespace Titan.GraphicsV2.Rendering.Commands
{
    internal enum RenderCommandTypes : uint
    {
        Invalid,
        SetRenderTarget,
        SetPixelShaderResource,
        SetVertexShaderResource,
        SetPixelShaderSamplers,
        SetVertexShaderSamplers,
        ClearRenderTarget
    }
}
