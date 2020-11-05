using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public interface IRenderContext
    {
        void ClearRenderTargetView(RenderTargetView renderTargetView, in Color color);
        void ClearDepthStencilView(DepthStencilView depthStencilView);
        void SetViewport(Viewport viewport);
        void SetBlendState(BlendState blendState);
        void SetVertexShaderConstantBuffer(IConstantBuffer constantBuffer, uint slot = 0);
        void SetPixelShaderConstantBuffer(IConstantBuffer constantBuffer, uint slot = 0);
        void SetVertexBuffer(IVertexBuffer vertexBuffer, uint slot = 0u, uint offset = 0u);
        void SetIndexBuffer(IIndexBuffer indexBuffer, uint offset = 0u);
        void SetRenderTarget(RenderTargetView renderTarget, DepthStencilView depthStencilView);
        void SetDepthStencilState(DepthStencilState depthStencilState);
        void SetRenderTarget(RenderTargetView renderTarget);
        void SetPixelShaderResource(ShaderResourceView resource, uint slot = 0u);
        void SetVertexShaderResource(ShaderResourceView resource, uint slot = 0u);
        void SetPixelShaderSampler(SamplerState samplerState, uint slot = 0u);
        void SetVertexShaderSampler(SamplerState samplerState, uint slot = 0u);
        void SetPixelShader(PixelShader pixelShader);
        void SetVertexShader(VertexShader vertexShader);
        void SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY topology);
        void SetInputLayout(InputLayout inputLayout);
        void DrawIndexed(uint indexCount, uint startIndexLocation = 0u, int baseVertexLocation = 0);
        void DrawIndexedInstanced(uint indexCountPerInstance, uint instanceCount, uint startIndexLocation = 0u, int baseVertexLocation = 0, uint startInstanceLocation = 0u);
        void ExecuteCommandList(in CommandList commandList, bool restoreContextState = false);
        
        unsafe void MapResource<T>(ID3D11Resource* resource, in T value) where T : unmanaged;
        unsafe void SetRenderTargets(ID3D11RenderTargetView** renderTargets, uint numViews, ID3D11DepthStencilView* depthStencilView);
        unsafe void SetPixelShaderResources(ID3D11ShaderResourceView** resources, uint numViews, uint startSlot = 0u);
        unsafe void SetVertexShaderResources(ID3D11ShaderResourceView** resources, uint numViews, uint startSlot = 0u);
    }
}
