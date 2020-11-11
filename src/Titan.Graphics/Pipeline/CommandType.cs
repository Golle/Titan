namespace Titan.Graphics.Pipeline
{
    internal enum CommandType
    {
        ClearRenderTarget,
        ClearDepthStencil,
        SetMultipleRenderTargets,
        SetMultipleRenderTargetAndDepthStencil,
        SetRenderTarget,
        SetRenderTargetAndDepthStencil,
        SetShaderProgram,
        SetVertexShaderResource,
        SetPixelShaderResource,
        SetVertexShaderSampler,
        SetPixelShaderSampler,
        Render,
        UnbindRenderTargets,  
        UnbindPixelShaderResources,
        UnbindVertexShaderResources
    }
}
