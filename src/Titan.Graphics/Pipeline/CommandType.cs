namespace Titan.Graphics.Pipeline
{
    internal enum CommandType
    {
        ClearRenderTarget,
        ClearDepthStencil,
        SetMultipleRenderTarget,
        SetRenderTarget,
        SetRenderTargetAndDepthStencil,
        SetShaderProgram,
        SetVertexShaderResource,
        SetPixelShaderResource,
        SetVertexShaderSampler,
        SetPixelShaderSampler,
        Render
    }
}
