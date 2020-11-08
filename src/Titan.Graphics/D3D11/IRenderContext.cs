using Titan.Graphics.D3D11.State;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders1;
using Titan.Graphics.States;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public interface IRenderContext
    {
        void ClearRenderTargetView(in RenderTargetView renderTargetView, in Color color);
        void ClearDepthStencilView(in DepthStencilView depthStencilView);
        void SetViewport(Viewport viewport);
        void SetBlendState(BlendState blendState);
        void SetVertexShaderConstantBuffer(in ConstantBuffer constantBuffer, uint slot = 0);
        void SetPixelShaderConstantBuffer(in ConstantBuffer constantBuffer, uint slot = 0);
        void SetVertexBuffer(in VertexBuffer vertexBuffer, uint slot = 0u, uint offset = 0u);
        void SetIndexBuffer(in IndexBuffer indexBuffer, uint offset = 0u);
        void SetRenderTarget(in RenderTargetView renderTarget);
        void SetRenderTarget(in RenderTargetView renderTarget, in DepthStencilView depthStencilView);
        void SetDepthStencilState(in DepthStencilState depthStencilState);
        void SetPixelShaderResource(in ShaderResourceView resource, uint slot = 0u);
        void SetVertexShaderResource(in ShaderResourceView resource, uint slot = 0u);
        void SetPixelShaderSampler(in SamplerState samplerState, uint slot = 0u);
        void SetVertexShaderSampler(in SamplerState samplerState, uint slot = 0u);
        void SetPixelShader(in PixelShader pixelShader);
        void SetVertexShader(in VertexShader vertexShader);
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
